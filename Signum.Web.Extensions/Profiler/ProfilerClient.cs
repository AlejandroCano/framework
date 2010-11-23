﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using Signum.Entities.Authorization;
using Signum.Engine.Authorization;
using System.Reflection;
using Signum.Web.Operations;
using Signum.Entities;
using System.Web.Mvc;
using Signum.Web.Properties;
using System.Diagnostics;
using Signum.Engine;
using Signum.Entities.Basics;
using Signum.Entities.Reflection;
using Signum.Entities.Operations;
using System.Linq.Expressions;
using Signum.Engine.Maps;
using System.Web.Routing;
using System.Web.Mvc.Html;

namespace Signum.Web.Profiler
{
    public static class ProfileClient
    {
        public static string ViewPath = "profiler/Views/";

        public static void Start()
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                AssemblyResourceManager.RegisterAreaResources(
                    new AssemblyResourceStore(typeof(ProfileClient), "/profiler/", "Signum.Web.Extensions.Profiler."));

                RouteTable.Routes.InsertRouteAt0("profiler/{resourcesFolder}/{*resourceName}",
                    new { controller = "Resources", action = "Index", area = "profiler" },
                    new { resourcesFolder = new InArray(new string[] { "Scripts", "Content", "Images" }) });
            }
        }

        public static MvcHtmlString ProfilerEntry(this HtmlHelper htmlHelper, string linkText,  string indices)
        {
            return htmlHelper.ActionLink(linkText, "HeavyRoute", new { indices }); 
        }
    }
}
