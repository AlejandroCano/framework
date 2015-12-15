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
    using Signum.Entities;
    
    #line 1 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
    using Signum.Entities.SMS;
    
    #line default
    #line hidden
    
    #line 4 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
    using Signum.Utilities;
    
    #line default
    #line hidden
    
    #line 3 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
    using Signum.Web;
    
    #line default
    #line hidden
    
    #line 2 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
    using Signum.Web.SMS;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/SMS/Views/SMSTemplateMessage.cshtml")]
    public partial class _SMS_Views_SMSTemplateMessage_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _SMS_Views_SMSTemplateMessage_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 6 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
 using (var tc = Html.TypeContext<SMSTemplateMessageEntity>())
{
    tc.LabelColumns = new BsColumn(4);

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"sf-sms-template-message\"");

WriteLiteral(">\r\n        <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" class=\"sf-tab-title\"");

WriteAttribute("value", Tuple.Create(" value=\"", 296), Tuple.Create("\"", 339)
            
            #line 10 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
, Tuple.Create(Tuple.Create("", 304), Tuple.Create<System.Object, System.Int32>(tc.Value.CultureInfo?.ToString()
            
            #line default
            #line hidden
, 304), false)
);

WriteLiteral(" />\r\n");

WriteLiteral("        ");

            
            #line 11 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
   Write(Html.EntityCombo(tc, e => e.CultureInfo, vl =>
        {
            vl.LabelText = SMSCharactersMessage.Language.NiceToString();
        }));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n        <div");

WriteLiteral(" class=\"sf-sms-edit-container\"");

WriteLiteral("> \r\n");

WriteLiteral("            ");

            
            #line 17 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
       Write(Html.ValueLine(tc, s => s.Message, vl =>
            {
                vl.ValueLineType = ValueLineType.TextArea;
                vl.ValueHtmlProps["cols"] = "30";
                vl.ValueHtmlProps["rows"] = "6";
                vl.ValueHtmlProps["class"] = "sf-sms-msg-text";
            }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"sf-sms-characters-left\"");

WriteLiteral(">\r\n                <p>\r\n                    <span>");

            
            #line 26 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
                     Write(SMSCharactersMessage.RemainingCharacters.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</span>: <span");

WriteLiteral(" class=\"sf-sms-chars-left\"");

WriteLiteral("></span>\r\n                </p>\r\n            </div>\r\n            <div>\r\n          " +
"      <input");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"btn sf-button sf-sms-remove-chars\"");

WriteAttribute("value", Tuple.Create(" value=\"", 1201), Tuple.Create("\"", 1270)
            
            #line 30 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
      , Tuple.Create(Tuple.Create("", 1209), Tuple.Create<System.Object, System.Int32>(SMSCharactersMessage.RemoveNonValidCharacters.NiceToString()
            
            #line default
            #line hidden
, 1209), false)
);

WriteLiteral(" />\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 34 "..\..\SMS\Views\SMSTemplateMessage.cshtml"
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
