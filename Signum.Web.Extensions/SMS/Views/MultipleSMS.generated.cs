﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
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
    
    #line 1 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Engine;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 2 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Entities.SMS;
    
    #line default
    #line hidden
    
    #line 5 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Utilities;
    
    #line default
    #line hidden
    
    #line 3 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Web;
    
    #line default
    #line hidden
    
    #line 4 "..\..\SMS\Views\MultipleSMS.cshtml"
    using Signum.Web.SMS;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/SMS/Views/MultipleSMS.cshtml")]
    public partial class _SMS_Views_MultipleSMS_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _SMS_Views_MultipleSMS_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

            
            #line 8 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.ScriptCss("~/SMS/Content/SMS.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 10 "..\..\SMS\Views\MultipleSMS.cshtml"
 using (var e = Html.TypeContext<MultipleSMSModel>())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"sf-sms-edit-container\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 13 "..\..\SMS\Views\MultipleSMS.cshtml"
   Write(Html.ValueLine(e, s => s.Message, vl =>
        {
            vl.ValueLineType = ValueLineType.TextArea;
            vl.ValueHtmlProps["cols"] = "30";
            vl.ValueHtmlProps["rows"] = "6";
            vl.ValueHtmlProps["class"] = "sf-sms-msg-text";
        }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"sf-sms-characters-left\"");

WriteLiteral(">\r\n            <p>\r\n                <span>");

            
            #line 22 "..\..\SMS\Views\MultipleSMS.cshtml"
                 Write(SMSCharactersMessage.RemainingCharacters.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</span>: <span");

WriteLiteral(" class=\"sf-sms-chars-left\"");

WriteLiteral("></span>\r\n            </p>\r\n        </div>\r\n        <div>\r\n            <input");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"sf-button sf-sms-remove-chars\"");

WriteLiteral(" value=\"Remove non valid characters\"");

WriteLiteral(" />\r\n        </div>\r\n    </div>\r\n");

WriteLiteral("    <br />\r\n");

            
            #line 30 "..\..\SMS\Views\MultipleSMS.cshtml"
    
            
            #line default
            #line hidden
            
            #line 30 "..\..\SMS\Views\MultipleSMS.cshtml"
Write(Html.ValueLine(e, s => s.From));

            
            #line default
            #line hidden
            
            #line 30 "..\..\SMS\Views\MultipleSMS.cshtml"
                                   
}

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
