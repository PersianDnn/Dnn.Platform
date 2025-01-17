#region Copyright
// 
// DotNetNuke� - http://www.dotnetnuke.com
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
#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Client.ClientResourceManagement;

#endregion

namespace DotNetNuke.Web.UI.WebControls.Internal
{
    ///<remarks>
    /// This control is only for internal use, please don't reference it in any other place as it may be removed in future.
    /// </remarks>
    public class DnnDatePicker : TextBox
    {
        protected virtual string Format => "yyyy-MM-dd";
        protected virtual string ClientFormat => "YYYY-MM-DD";

        public DateTime? SelectedDate {
            get
            {
                DateTime value;
                if (!string.IsNullOrEmpty(Text) && DateTime.TryParse(Text, out value))
                {
                    return value;
                }

                return null;
            }
            set
            {
                Text = value?.ToString(Format) ?? string.Empty;
            }
        }

        public DateTime MinDate { get; set; } = new DateTime(1900, 1, 1);

        public DateTime MaxDate { get; set; } = DateTime.MaxValue;


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            JavaScript.RequestRegistration(CommonJs.jQuery);

            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/components/DatePicker/moment.min.js");
            //START persian-dnnsoftware
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/components/DatePicker/persian.datepicker.js");
            }
            else
            {
                ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/components/DatePicker/pikaday.js");
            }
            //END persian-dnnsoftware
            ClientResourceManager.RegisterScript(Page, "~/Resources/Shared/components/DatePicker/pikaday.jquery.js");
            //START persian-dnnsoftware
            if (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                ClientResourceManager.RegisterStyleSheet(Page, "~/Resources/Shared/components/DatePicker/persian.datepicker.css");
            }
            else
            {
                ClientResourceManager.RegisterStyleSheet(Page, "~/Resources/Shared/components/DatePicker/pikaday.css");
            }
            //END persian-dnnsoftware

            RegisterClientResources();
        }

        protected virtual IDictionary<string, object> GetSettings()
        {
            return new Dictionary<string, object>
            {
                {"minDate", MinDate > DateTime.MinValue ? $"$new Date('{MinDate.ToString(Format, CultureInfo.InvariantCulture)}')$" : ""},
                {"maxDate", MaxDate > DateTime.MinValue ? $"$new Date('{MaxDate.ToString(Format, CultureInfo.InvariantCulture)}')$" : ""},
                {"format", ClientFormat }
            };
        } 

        private void RegisterClientResources()
        {
            var settings = Json.Serialize(GetSettings()).Replace("\"$", "").Replace("$\"", "");
            var script = $"$('#{ClientID}').pikaday({settings});";

            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "DnnDatePicker" + ClientID, script, true);
        }
    }
}