﻿#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2018
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

namespace DotNetNuke.Entities.Host
{
    /// <summary>
    /// Do not implement.  This interface is meant for reference and unit test purposes only.
    /// There is no guarantee that this interface will not change.
    /// </summary>
    public interface IIPFilterController
    {
        int AddIPFilter(IPFilterInfo ipFilter);

        void UpdateIPFilter(IPFilterInfo ipFilter);

        void DeleteIPFilter(IPFilterInfo ipFilter);

        IPFilterInfo GetIPFilter(int ipFilter);

        IList<IPFilterInfo> GetIPFilters();

        [Obsolete("deprecated with 7.1.0 - please use IsIPBanned instead. Scheduled removal in v10.0.0.")]
        void IsIPAddressBanned(string ipAddress);

        bool IsIPBanned(string ipAddress);

        bool IsAllowableDeny(string ipAddress, IPFilterInfo ipFilter);

        bool CanIPStillAccess(string myip, IList<IPFilterInfo> filterList);
    }
}