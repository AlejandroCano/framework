﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.ControlPanel.Views.Admin
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    
    #line 3 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
    using System.Reflection;
    
    #line default
    #line hidden
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using Signum.Entities;
    
    #line 1 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
    using Signum.Entities.ControlPanel;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    #line 2 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
    using Signum.Web.ControlPanel;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/ControlPanel/Views/Admin/ControlPanelAdmin.cshtml")]
    public partial class ControlPanelAdmin : System.Web.Mvc.WebViewPage<dynamic>
    {
        public ControlPanelAdmin()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
Write(Html.ScriptCss("~/ControlPanel/Content/ControlPanel.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n\r\n");

            
            #line 8 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
 using (var tc = Html.TypeContext<ControlPanelDN>())
{
    using (var sc = tc.SubContext())
    {
        sc.FormGroupStyle = FormGroupStyle.Basic;
    

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"form-vertical\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 17 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
           Write(Html.ValueLine(sc, cp => cp.DisplayName));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"col-sm-3\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 20 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
           Write(Html.ValueLine(sc, cp => cp.HomePagePriority));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n             <div");

WriteLiteral(" class=\"col-sm-3\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 23 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
           Write(Html.ValueLine(sc, cp => cp.AutoRefreshPeriod));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 28 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
           Write(Html.EntityLine(sc, cp => cp.Owner, el => el.Create = false));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"col-sm-6\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 31 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
           Write(Html.EntityLine(sc, cp => cp.EntityType, el => { el.AutocompleteUrl = Url.Action("TypeAutocomplete", "Finder"); }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 35 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
    
    }
    
    
            
            #line default
            #line hidden
            
            #line 38 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
Write(Html.GridRepeater(tc, cp => cp.Parts, grid =>
        {
            grid.PartialViewName = ControlPanelClient.AdminViewPrefix.Formato("PanelPartViewAdmin");
            grid.AttachFunction = ControlPanelClient.Module["attachGridControl"](grid,
               Url.Action("AddNewPart", "ControlPanel"),
               ControlPanelClient.PanelPartViews.Keys.Select(t => t.ToJsTypeInfo(isSearch: false, prefix: grid.Prefix)).ToArray());
        }));

            
            #line default
            #line hidden
            
            #line 44 "..\..\ControlPanel\Views\Admin\ControlPanelAdmin.cshtml"
          ;
}

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

        }
    }
}
#pragma warning restore 1591
