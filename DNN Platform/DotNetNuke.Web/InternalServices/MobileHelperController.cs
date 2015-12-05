﻿#region Copyright
//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2016
// by Ash Prasad
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Models;

namespace DotNetNuke.Web.InternalServices
{
    [RequireJwt]
    public class MobileHelperController : DnnApiController
    {
        private readonly string _dnnVersion = Globals.FormatVersion(DotNetNukeContext.Current.Application.Version, false);

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ModuleDetails(string moduleList)
        {
            var siteDetails = new SiteDetail
            {
                SiteName = PortalSettings.PortalName,
                DnnVersion = _dnnVersion,
                IsHost = UserInfo.IsSuperUser,
                IsAdmin = UserInfo.IsInRole("Administrators")
            };

            foreach (var moduleName in (moduleList ?? "").Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries))
            {
                var modulesCollection = GetTabModules((moduleName ?? "").Trim())
                    .Where(tabmodule => TabPermissionController.CanViewPage(tabmodule.TabInfo) &&
                                        ModulePermissionController.CanViewModule(tabmodule.ModuleInfo));
                foreach (var tabmodule in modulesCollection)
                {
                    var moduleDetail = new ModuleDetail
                    {
                        ModuleName = moduleName,
                        ModuleVersion = tabmodule.ModuleVersion
                    };

                    moduleDetail.ModuleInstances.Add(new ModuleInstance
                    {
                        TabId = tabmodule.TabInfo.TabID,
                        ModuleId = tabmodule.ModuleInfo.ModuleID,
                        PageName = tabmodule.TabInfo.TabName,
                        PagePath = tabmodule.TabInfo.TabPath
                    });
                    siteDetails.Modules.Add(moduleDetail);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, siteDetails);
        }

        #region private methods

        private static IEnumerable<TabModule> GetTabModules(string moduleName)
        {
            var portalId = PortalController.Instance.GetCurrentPortalSettings().PortalId;
            var desktopModule = DesktopModuleController.GetDesktopModuleByModuleName(moduleName, portalId);
            if (desktopModule != null)
            {

                var cacheKey = string.Format(DataCache.DesktopModuleCacheKey, portalId) + "_" +
                               desktopModule.DesktopModuleID;
                var args = new CacheItemArgs(cacheKey, DataCache.DesktopModuleCacheTimeOut,
                                             DataCache.DesktopModuleCachePriority, portalId, desktopModule);

                return CBO.GetCachedObject<IList<TabModule>>(args, GetTabModulesCallback);
            }

            return new List<TabModule>();
        }

        private static object GetTabModulesCallback(CacheItemArgs cacheItemArgs)
        {
            var tabModules = new List<TabModule>();

            var portalId = (int)cacheItemArgs.ParamList[0];
            var desktopModule = (DesktopModuleInfo)cacheItemArgs.ParamList[1];

            var tabController = new TabController();
            var tabsWithModule = tabController.GetTabsByPackageID(portalId, desktopModule.PackageID, false);
            var allPortalTabs = tabController.GetTabsByPortal(portalId);
            IDictionary<int, TabInfo> tabsInOrder = new Dictionary<int, TabInfo>();

            //must get each tab, they parent may not exist
            foreach (var tab in allPortalTabs.Values)
            {
                AddChildTabsToList(tab, ref allPortalTabs, ref tabsWithModule, ref tabsInOrder);
            }

            foreach (var tab in tabsInOrder.Values)
            {
                tabModules.AddRange(
                    tab.ChildModules.Values.Where(
                        childModule => childModule.DesktopModuleID == desktopModule.DesktopModuleID)
                       .Select(childModule => new TabModule
                       {
                           TabInfo = tab,
                           ModuleInfo = childModule,
                           ModuleVersion = desktopModule.Version
                       }));
            }

            return tabModules;
        }

        private static void AddChildTabsToList(TabInfo currentTab, ref TabCollection allPortalTabs,
            ref IDictionary<int, TabInfo> tabsWithModule, ref IDictionary<int, TabInfo> tabsInOrder)
        {
            if (tabsWithModule.ContainsKey(currentTab.TabID) && !tabsInOrder.ContainsKey(currentTab.TabID))
            {
                //add current tab
                tabsInOrder.Add(currentTab.TabID, currentTab);
                //add children of current tab
                foreach (var tab in allPortalTabs.WithParentId(currentTab.TabID))
                {
                    AddChildTabsToList(tab, ref allPortalTabs, ref tabsWithModule, ref tabsInOrder);
                }
            }
        }

        #endregion
    }
}