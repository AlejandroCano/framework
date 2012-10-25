﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
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
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using Signum.Utilities;
    using Signum.Entities;
    using Signum.Web;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Caching;
    using System.Web.DynamicData;
    using System.Web.SessionState;
    using System.Web.Profile;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Web.UI.HtmlControls;
    using System.Xml.Linq;
    using Signum.Web.Properties;
    using Signum.Entities.ControlPanel;
    using Signum.Web.ControlPanel;
    using Signum.Entities.DynamicQuery;
    using Signum.Entities.Chart;
    using Signum.Web.Chart;
    using Signum.Engine.Chart;
    using Signum.Engine.DynamicQuery;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("MvcRazorClassGenerator", "1.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/ControlPanel/Views/UserChartPart.cshtml")]
    public class _Page_ControlPanel_Views_UserChartPart_cshtml : System.Web.Mvc.WebViewPage<PanelPart>
    {


        public _Page_ControlPanel_Views_UserChartPart_cshtml()
        {
        }
        protected System.Web.HttpApplication ApplicationInstance
        {
            get
            {
                return ((System.Web.HttpApplication)(Context.ApplicationInstance));
            }
        }
        public override void Execute()
        {











Write(Html.ScriptsJs("~/Chart/Scripts/SF_Chart.js",
                "~/Chart/Scripts/SF_Chart_Utils.js",
                "~/scripts/d3.v2.min.js",
                "~/scripts/colorbrewer.js"));

WriteLiteral("\r\n");


Write(Html.ScriptCss("~/Chart/Content/SF_Chart.css"));

WriteLiteral("\r\n");


   
    UserChartDN uc = ((UserChartPartDN)Model.Content).UserChart;
    ChartRequest request = uc.ToRequest();

    using (var crc = new TypeContext<ChartRequest>(request, "r{0}c{1}".Formato(Model.Row, Model.Column)))
    {
        ResultTable resultTable = ChartLogic.ExecuteChart(request);



WriteLiteral("    <div id=\"");


        Write(crc.Compose("sfChartControl"));

WriteLiteral("\" class=\"sf-search-control sf-chart-control\" data-prefix=\"");


                                                                                                Write(crc.ControlID);

WriteLiteral("\">\r\n        <div style=\"display: none\">\r\n            ");


       Write(Html.HiddenRuntimeInfo(crc));

WriteLiteral("\r\n            ");


       Write(Html.Hidden(crc.Compose("sfOrders"), request.Orders.IsNullOrEmpty() ? "" :
                    (request.Orders.ToString(oo => (oo.OrderType == OrderType.Ascending ? "" : "-") + oo.Token.FullKey(), ";") + ";")));

WriteLiteral("\r\n");


              
        ViewData[ViewDataKeys.QueryDescription] = DynamicQueryManager.Current.QueryDescription(request.QueryName);
        ViewData[ViewDataKeys.FilterOptions] = request.Filters.Select(f => new FilterOption { Token = f.Token, Operation = f.Operation, Value = f.Value }).ToList();
            

WriteLiteral("            ");


       Write(Html.Partial(Navigator.Manager.FilterBuilderView, crc));

WriteLiteral("\r\n            <div id=\"");


                Write(crc.Compose("sfChartBuilderContainer"));

WriteLiteral("\">\r\n                ");


           Write(Html.Partial(ChartClient.ChartBuilderView, crc));

WriteLiteral("\r\n            </div>\r\n            <script type=\"text/javascript\">\r\n              " +
"          $(\'#");


                       Write(crc.Compose("sfChartBuilderContainer"));

WriteLiteral("\').chartBuilder($.extend({ prefix: \'");


                                                                                                  Write(crc.ControlID);

WriteLiteral("\'}, ");


                                                                                                                    Write(MvcHtmlString.Create(uc.ToJS()));

WriteLiteral("));\r\n            </script>\r\n        </div>\r\n        <div id=\"");


            Write(crc.Compose("sfChartContainer"));

WriteLiteral("\">\r\n            <div class=\"sf-chart-container\" \r\n                    data-open-u" +
"rl=\"");


                               Write(Url.Action<ChartController>(cc => cc.OpenSubgroup(crc.ControlID)));

WriteLiteral("\" \r\n                    data-fullscreen-url=\"");


                                     Write(Url.Action<ChartController>(cc => cc.FullScreen(crc.ControlID)));

WriteLiteral("\"\r\n                    data-json=\"");


                          Write(Html.Json(ChartUtils.DataJson(crc.Value, resultTable)).ToString());

WriteLiteral("\">\r\n            </div>\r\n        </div>\r\n    </div>\r\n");


    
        MvcHtmlString divSelector = MvcHtmlString.Create("#" + crc.Compose("sfChartContainer") + " > .sf-chart-container");
   

WriteLiteral("        <script type=\"text/javascript\">\r\n            (function () {\r\n            " +
"    var $myChart = SF.Chart.getFor(\'");


                                           Write(crc.ControlID);

WriteLiteral("\');\r\n                $myChart.reDraw();\r\n\r\n                $(\"#\" + SF.compose(\"");


                               Write(crc.ControlID);

WriteLiteral("\", \"sfFullScreen\")).on(\"mousedown\", function (e) {\r\n                    $myChart." +
"fullScreen(e);\r\n                });\r\n            })();\r\n        </script>\r\n");


        
    }



        }
    }
}
