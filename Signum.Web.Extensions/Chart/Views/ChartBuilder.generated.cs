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
    
    #line 4 "..\..\Chart\Views\ChartBuilder.cshtml"
    using System.Configuration;
    
    #line default
    #line hidden
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
    
    #line 10 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Engine.Chart;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Engine.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Entities;
    
    #line default
    #line hidden
    
    #line 7 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Entities.Chart;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Entities.DynamicQuery;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Entities.Reflection;
    
    #line default
    #line hidden
    
    #line 9 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Utilities;
    
    #line default
    #line hidden
    
    #line 1 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Web;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Chart\Views\ChartBuilder.cshtml"
    using Signum.Web.Chart;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Chart/Views/ChartBuilder.cshtml")]
    public partial class _Chart_Views_ChartBuilder_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Chart_Views_ChartBuilder_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 11 "..\..\Chart\Views\ChartBuilder.cshtml"
 using (var chart = Html.TypeContext<IChartBase>())
{
    QueryDescription queryDescription = (QueryDescription)ViewData[ViewDataKeys.QueryDescription];


            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteAttribute("id", Tuple.Create(" id=\"", 453), Tuple.Create("\"", 490)
            
            #line 15 "..\..\Chart\Views\ChartBuilder.cshtml"
, Tuple.Create(Tuple.Create("", 458), Tuple.Create<System.Object, System.Int32>(chart.Compose("sfChartBuilder")
            
            #line default
            #line hidden
, 458), false)
);

WriteLiteral(" class=\"row sf-chart-builder\"");

WriteLiteral(">\r\n\r\n        <div");

WriteLiteral(" class=\"col-lg-2\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"sf-chart-type panel panel-default\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"panel-heading\"");

WriteLiteral(">\r\n                    <h3");

WriteLiteral(" class=\"panel-title\"");

WriteLiteral(">");

            
            #line 20 "..\..\Chart\Views\ChartBuilder.cshtml"
                                       Write(typeof(ChartScriptEntity).NiceName());

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n");

            
            #line 21 "..\..\Chart\Views\ChartBuilder.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 21 "..\..\Chart\Views\ChartBuilder.cshtml"
                     using (var csc = chart.SubContext(c => c.ChartScript))
                    {
                        
            
            #line default
            #line hidden
            
            #line 23 "..\..\Chart\Views\ChartBuilder.cshtml"
                   Write(Html.Hidden(csc.Compose("sfRuntimeInfo"), csc.RuntimeInfo().ToString(), new { @class = "sf-chart-type-value" }));

            
            #line default
            #line hidden
            
            #line 23 "..\..\Chart\Views\ChartBuilder.cshtml"
                                                                                                                                        
                    }

            
            #line default
            #line hidden
WriteLiteral("                    ");

            
            #line 25 "..\..\Chart\Views\ChartBuilder.cshtml"
               Write(Html.Hidden(chart.Compose("GroupResults"), chart.Value.GroupResults, new { @class = "sf-chart-group-results" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"panel-body\"");

WriteLiteral(">\r\n");

            
            #line 28 "..\..\Chart\Views\ChartBuilder.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 28 "..\..\Chart\Views\ChartBuilder.cshtml"
                     foreach (var group in ChartUtils.PackInGroups(ChartScriptLogic.Scripts.Value.Values, 4))
                    {
                        foreach (var script in group)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <div");

WriteAttribute("class", Tuple.Create(" class=\"", 1460), Tuple.Create("\"", 1544)
            
            #line 32 "..\..\Chart\Views\ChartBuilder.cshtml"
, Tuple.Create(Tuple.Create("", 1468), Tuple.Create<System.Object, System.Int32>(ChartClient.ChartTypeImgClass(chart.Value, chart.Value.ChartScript, script)
            
            #line default
            #line hidden
, 1468), false)
);

WriteLiteral("\r\n                                 data-related=\"");

            
            #line 33 "..\..\Chart\Views\ChartBuilder.cshtml"
                                           Write(new RuntimeInfo(script).ToString());

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1632), Tuple.Create("\"", 1695)
            
            #line 33 "..\..\Chart\Views\ChartBuilder.cshtml"
              , Tuple.Create(Tuple.Create("", 1640), Tuple.Create<System.Object, System.Int32>(script.ToString() + "\r\n" + script.ColumnsStructure
            
            #line default
            #line hidden
, 1640), false)
);

WriteLiteral(">\r\n                                <img");

WriteAttribute("src", Tuple.Create(" src=\"", 1735), Tuple.Create("\"", 1914)
            
            #line 34 "..\..\Chart\Views\ChartBuilder.cshtml"
, Tuple.Create(Tuple.Create("", 1741), Tuple.Create<System.Object, System.Int32>(script.Icon == null ? Url.Content("~/Chart/Images/unknown.png") : Url.Action((Signum.Web.Files.FileController fc) => fc.Download(new RuntimeInfo(script.Icon).ToString()))
            
            #line default
            #line hidden
, 1741), false)
);

WriteLiteral(" />\r\n                            </div>\r\n");

            
            #line 36 "..\..\Chart\Views\ChartBuilder.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                        <div");

WriteLiteral(" class=\"clearall\"");

WriteLiteral(">\r\n                        </div>\r\n");

            
            #line 39 "..\..\Chart\Views\ChartBuilder.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </div>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"col-lg-10\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"sf-chart-tokens panel panel-default\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"panel-heading\"");

WriteLiteral(">\r\n                    <h3");

WriteLiteral(" class=\"panel-title\"");

WriteLiteral(">");

            
            #line 46 "..\..\Chart\Views\ChartBuilder.cshtml"
                                       Write(ChartMessage.Chart_ChartSettings.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("</h3>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"panel-body table-responsive\"");

WriteLiteral(">\r\n                    <table");

WriteLiteral(" class=\"table\"");

WriteLiteral(">\r\n                        <thead>\r\n                            <tr>\r\n           " +
"                     <th");

WriteLiteral(" class=\"sf-chart-token-narrow\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 53 "..\..\Chart\Views\ChartBuilder.cshtml"
                               Write(ChartMessage.Chart_Dimension.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </th>\r\n                                <th");

WriteLiteral(" class=\"\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 56 "..\..\Chart\Views\ChartBuilder.cshtml"
                               Write(ChartMessage.Chart_Group.NiceToString());

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </th>\r\n                                <th");

WriteLiteral(" class=\"sf-chart-token-wide\"");

WriteLiteral(">\r\n                                    Token\r\n                                </t" +
"h>\r\n                            </tr>\r\n                        </thead>\r\n       " +
"                 <tbody>\r\n");

            
            #line 64 "..\..\Chart\Views\ChartBuilder.cshtml"
                            
            
            #line default
            #line hidden
            
            #line 64 "..\..\Chart\Views\ChartBuilder.cshtml"
                             foreach (var column in chart.TypeElementContext(a => a.Columns))
                    {
                            
            
            #line default
            #line hidden
            
            #line 66 "..\..\Chart\Views\ChartBuilder.cshtml"
                       Write(Html.HiddenRuntimeInfo(column));

            
            #line default
            #line hidden
            
            #line 66 "..\..\Chart\Views\ChartBuilder.cshtml"
                                                           
                            
            
            #line default
            #line hidden
            
            #line 67 "..\..\Chart\Views\ChartBuilder.cshtml"
                       Write(Html.EmbeddedControl(column, c => c, ec => ec.ViewData[ViewDataKeys.QueryDescription] = queryDescription));

            
            #line default
            #line hidden
            
            #line 67 "..\..\Chart\Views\ChartBuilder.cshtml"
                                                                                                                                      
                    }

            
            #line default
            #line hidden
WriteLiteral("                        </tbody>\r\n                    </table>\r\n                <" +
"/div>\r\n                <textarea");

WriteLiteral(" class=\"sf-chart-currentScript\"");

WriteLiteral(" style=\"display:none\"");

WriteLiteral(" data-url=\"");

            
            #line 72 "..\..\Chart\Views\ChartBuilder.cshtml"
                                                                                   Write(Navigator.NavigateRoute(chart.Value.ChartScript));

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 73 "..\..\Chart\Views\ChartBuilder.cshtml"
               Write(chart.Value.ChartScript.Script);

            
            #line default
            #line hidden
WriteLiteral("\r\n                </textarea>\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"sf-chart-parameters panel panel-default\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"panel-body form-vertical\"");

WriteLiteral(">\r\n");

            
            #line 78 "..\..\Chart\Views\ChartBuilder.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 78 "..\..\Chart\Views\ChartBuilder.cshtml"
                     foreach (var gr in chart.TypeElementContext(c => c.Parameters).OrderBy(c => chart.Value.Parameters.IndexOf(c.Value)).GroupsOf(6))
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <div");

WriteLiteral(" class=\"row\"");

WriteLiteral(">\r\n");

            
            #line 81 "..\..\Chart\Views\ChartBuilder.cshtml"
                            
            
            #line default
            #line hidden
            
            #line 81 "..\..\Chart\Views\ChartBuilder.cshtml"
                             foreach (var pc in gr)
                            {
                                pc.FormGroupStyle = FormGroupStyle.Basic;

            
            #line default
            #line hidden
WriteLiteral("                                <div");

WriteLiteral(" class=\"col-sm-2\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 85 "..\..\Chart\Views\ChartBuilder.cshtml"
                               Write(Html.HiddenLine(pc, ct => ct.Name));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 86 "..\..\Chart\Views\ChartBuilder.cshtml"
                               Write(Html.ValueLine(pc, ct => ct.Value, vl => ChartClient.SetupParameter(vl, pc.Value)));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </div>\r\n");

            
            #line 88 "..\..\Chart\Views\ChartBuilder.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </div>\r\n");

            
            #line 90 "..\..\Chart\Views\ChartBuilder.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 95 "..\..\Chart\Views\ChartBuilder.cshtml"
}
            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
