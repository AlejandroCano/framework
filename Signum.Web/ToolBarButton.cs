﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using System.Web.Mvc;
using System.Web;
using Signum.Web.Properties;

namespace Signum.Web
{
    public class ToolBarButton
    {
        public string Id { get; set; }
        public string ImgSrc { get; set; }
        public string Text { get; set; }
        public string AltText { get; set; }
        public string OnClick { get; set; }
        
        public static string DefaultEntityDivCssClass = "ButtonDiv";
        public static string DefaultQueryCssClass = "QueryButton";

        private string divCssClass;
        public string DivCssClass 
        { 
            get { return divCssClass; } 
            set { divCssClass = value; } 
        }

        private bool enabled = true;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        Dictionary<string, object> htmlProps = new Dictionary<string, object>(0);
        public Dictionary<string, object> HtmlProps
        {
            get { return htmlProps; }
        }

        public virtual string ToString(HtmlHelper helper)
        {
            HtmlProps.Add("title", AltText ?? "");

            if (ImgSrc.HasText())
            {
                if (HtmlProps.ContainsKey("style"))
                    HtmlProps["style"] = "background:transparent url(" + ImgSrc + ")  no-repeat scroll left top; " + HtmlProps["style"].ToString();
                else
                    HtmlProps["style"] = "background:transparent url(" + ImgSrc + ")  no-repeat scroll left top;";

                return helper.Link(Id, "", DivCssClass, HtmlProps);
            }
            else
            {
                if (enabled)
                {
                    HtmlProps.Add("onclick", OnClick);
                }
                else
                    DivCssClass = DivCssClass + " disabled"; 
                return helper.Link(Id, Text, DivCssClass, HtmlProps);
            }
        }
    }
}
