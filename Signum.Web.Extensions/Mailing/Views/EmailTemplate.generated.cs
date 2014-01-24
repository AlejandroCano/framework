﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Signum.Web.Extensions.Mailing.Views
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
    
    #line 3 "..\..\Mailing\Views\EmailTemplate.cshtml"
    using Signum.Engine.Basics;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Mailing\Views\EmailTemplate.cshtml"
    using Signum.Engine.DynamicQuery;
    
    #line default
    #line hidden
    using Signum.Entities;
    
    #line 1 "..\..\Mailing\Views\EmailTemplate.cshtml"
    using Signum.Entities.Mailing;
    
    #line default
    #line hidden
    using Signum.Utilities;
    using Signum.Web;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Mailing/Views/EmailTemplate.cshtml")]
    public partial class EmailTemplate : System.Web.Mvc.WebViewPage<dynamic>
    {
        public EmailTemplate()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\Mailing\Views\EmailTemplate.cshtml"
Write(Html.ScriptCss("~/Mailing/Content/SF_Mailing.css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 6 "..\..\Mailing\Views\EmailTemplate.cshtml"
Write(Html.ScriptsJs("~/Scripts/ckeditor/ckeditor.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 7 "..\..\Mailing\Views\EmailTemplate.cshtml"
Write(Html.ScriptsJs("~/Mailing/Scripts/SF_Mailing.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 9 "..\..\Mailing\Views\EmailTemplate.cshtml"
 using (var ec = Html.TypeContext<EmailTemplateDN>())
{

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" style=\"float: left\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 12 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.Name));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 13 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.EntityCombo(ec, e => e.SystemEmail));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 14 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.EntityLine(ec, e => e.Query));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 15 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.EntityLine(ec, e => e.SmtpConfiguration));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 16 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.EditableMessage));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 17 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.DisableAuthorization));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 19 "..\..\Mailing\Views\EmailTemplate.cshtml"
    
    if(!ec.Value.IsNew)
    {

            
            #line default
            #line hidden
WriteLiteral("    <fieldset");

WriteLiteral(" style=\"float: left\"");

WriteLiteral(">\r\n        <legend>Active</legend>\r\n");

WriteLiteral("        ");

            
            #line 24 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.Active));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 25 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.StartDate));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 26 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.EndDate));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </fieldset>\r\n");

            
            #line 28 "..\..\Mailing\Views\EmailTemplate.cshtml"
    }
    

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"clearall\"");

WriteLiteral("></div>\r\n");

            
            #line 31 "..\..\Mailing\Views\EmailTemplate.cshtml"
        
    if (ec.Value.Query != null)
    {
        object queryName = QueryLogic.ToQueryName(ec.Value.Query.Key);
        ViewData[ViewDataKeys.QueryDescription] = DynamicQueryManager.Current.QueryDescription(queryName); //To be use inside query token controls
        
        
            
            #line default
            #line hidden
            
            #line 37 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.SendDifferentMessages));

            
            #line default
            #line hidden
            
            #line 37 "..\..\Mailing\Views\EmailTemplate.cshtml"
                                                         
        
            
            #line default
            #line hidden
            
            #line 38 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.EntityLineDetail(ec, e => e.From, el => el.PreserveViewData = true));

            
            #line default
            #line hidden
            
            #line 38 "..\..\Mailing\Views\EmailTemplate.cshtml"
                                                                                 
        
            
            #line default
            #line hidden
            
            #line 39 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.EntityRepeater(ec, e => e.Recipients, el => el.PreserveViewData = true));

            
            #line default
            #line hidden
            
            #line 39 "..\..\Mailing\Views\EmailTemplate.cshtml"
                                                                                     
    
        
            
            #line default
            #line hidden
            
            #line 41 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.EntityLine(ec, e => e.MasterTemplate));

            
            #line default
            #line hidden
            
            #line 41 "..\..\Mailing\Views\EmailTemplate.cshtml"
                                                   
        
            
            #line default
            #line hidden
            
            #line 42 "..\..\Mailing\Views\EmailTemplate.cshtml"
   Write(Html.ValueLine(ec, e => e.IsBodyHtml));

            
            #line default
            #line hidden
            
            #line 42 "..\..\Mailing\Views\EmailTemplate.cshtml"
                                              


            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"sf-email-replacements-container sf-tabs-repeater\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Mailing\Views\EmailTemplate.cshtml"
       Write(Html.EntityRepeater(ec, e => e.Messages, er =>
            {
                er.AttachFunction = "SF.TabsRepeater.attachTabRepeater";
                er.PreserveViewData = true;
            }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

            
            #line 51 "..\..\Mailing\Views\EmailTemplate.cshtml"
    }
}

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 54 "..\..\Mailing\Views\EmailTemplate.cshtml"
Write(Html.ScriptsJs("~/Mailing/Scripts/SF_TabsRepeater.js"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<script>\r\n    $(function () {\r\n        SF.Mailing.initReplacements();\r\n    })" +
";\r\n</script>\r\n");

        }
    }
}
#pragma warning restore 1591
