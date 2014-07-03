using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.DynamicQuery;
using Signum.Utilities;
using Signum.Entities;
using System.Web;
using System.Web.Mvc;
using Signum.Entities.Reflection;
using System.Linq.Expressions;
using System.Web.Mvc.Html;

namespace Signum.Web
{
    public class QuerySettings
    {
        public object QueryName { get; private set; }

        public Func<string> Title { get; set; }
        public Pagination Pagination { get; set; }
        public string WebQueryName { get; set; }

        public bool IsFindable { get; set; }

        public QuerySettings(object queryName)
        {
            this.QueryName = queryName;
            this.IsFindable = true;
        }

        public static List<FormatterRule> FormatRules { get; set; }
        public static List<EntityFormatterRule> EntityFormatRules { get; set; }

        public static Dictionary<PropertyRoute, Func<HtmlHelper, object, MvcHtmlString>> PropertyFormatters { get; set; }

        Dictionary<string, Func<HtmlHelper, object, MvcHtmlString>> formatters;
        public Dictionary<string, Func<HtmlHelper, object, MvcHtmlString>> Formatters
        {
            get { return formatters ?? (formatters = new Dictionary<string, Func<HtmlHelper, object, MvcHtmlString>>()); }
            set { formatters = value; }
        }

    
        static QuerySettings()
        {
            FormatRules = new List<FormatterRule>
            {
                new FormatterRule(c=>true, c=> (h,o) =>
                {
                    return o != null ? o.ToString().EncodeHtml() : MvcHtmlString.Empty;
                }){ WriteData = false },

                new FormatterRule(c => c.Type.UnNullify().IsEnum, c => (h,o) => 
                {
                    return o != null ? ((Enum)o).NiceToString().EncodeHtml() : MvcHtmlString.Empty;
                }){ WriteData = true },
                new FormatterRule(c => c.Type.UnNullify().IsLite(), c => (h,o) => 
                {
                    return h.LightEntityLine((Lite<IIdentifiable>)o, false);
                }),
                new FormatterRule(c=>c.Type.UnNullify() == typeof(DateTime), c => (h,o) => 
                {
                    return o != null ? ((DateTime)o).ToUserInterface().TryToString(c.Format).EncodeHtml() : MvcHtmlString.Empty;
                }){ WriteData = false },
                new FormatterRule(c=>c.Type.UnNullify() == typeof(TimeSpan), c => (h,o) => 
                {
                    return o != null ? ((TimeSpan)o).TryToString(c.Format).EncodeHtml() : MvcHtmlString.Empty;
                }){ WriteData = false },
                new FormatterRule(c=> Reflector.IsNumber(c.Type) && c.Unit == null, c => (h,o) => 
                {
                    return o != null? ((IFormattable)o).TryToString(c.Format).EncodeHtml(): MvcHtmlString.Empty;
                }){ WriteData = false },
                new FormatterRule(c=> Reflector.IsNumber(c.Type) && c.Unit.HasText(), c => (h,o) => 
                {
                    if (o != null)
                    {
                        string s = ((IFormattable)o).TryToString(c.Format);
                        if (c.Unit.HasText())
                            s += " " + c.Unit;
                        return s.EncodeHtml();
                    }
                    return MvcHtmlString.Empty;
                }),
                new FormatterRule(c=>c.Type.UnNullify() == typeof(bool), c => (h,o) => 
                {
                    return o != null ? new HtmlTag("input")
                        .Attr("type", "checkbox")
                        .Attr("style", "text-align:center")
                        .Attr("disabled", "disabled")
                        .Let(a => (bool)o ? a.Attr("checked", "checked") : a)
                        .ToHtml() : MvcHtmlString.Empty;
                }),
            };

            EntityFormatRules = new List<EntityFormatterRule>
            {
                new EntityFormatterRule(l => true, (h,l) => 
                {
                    if (Navigator.IsNavigable(l.EntityType, null, isSearch: true ))
                        return h.Href(Navigator.NavigateRoute(l.EntityType, l.Id), h.Encode(EntityControlMessage.View.NiceToString()));
                    else
                        return MvcHtmlString.Empty;
                }),
            };

            PropertyFormatters = new Dictionary<PropertyRoute, Func<HtmlHelper, object, MvcHtmlString>>();
        }

        public CellFormatter GetFormatter(Column column)
        {
            Func<HtmlHelper, object, MvcHtmlString> cf;
            if (formatters != null && formatters.TryGetValue(column.Name, out cf))
                return new CellFormatter { WriteData = true, Formatter = cf };

            PropertyRoute route = column.Token.GetPropertyRoute();
            if (route != null)
            {
                var formatter = QuerySettings.PropertyFormatters.TryGetC(route);
                if (formatter != null)
                    return new CellFormatter { WriteData = true, Formatter = formatter };
            }

            var last = FormatRules.Last(cfr => cfr.IsApplyable(column));

            return new CellFormatter { WriteData = last.WriteData, Formatter = last.Formatter(column) };
        }


        public static void RegisterPropertyFormat<T>(Expression<Func<T, object>> propertyRoute, Func<HtmlHelper, object, MvcHtmlString> formatter)
         where T : IRootEntity
        {
            PropertyFormatters.Add(PropertyRoute.Construct(propertyRoute), formatter);
        }
    }

    public class FormatterRule
    {
        public Func<Column, Func<HtmlHelper, object, MvcHtmlString>> Formatter { get; set; }
        public Func<Column, bool> IsApplyable { get; set; }
        public bool WriteData = true;

        public FormatterRule(Func<Column, bool> isApplyable, Func<Column, Func<HtmlHelper, object, MvcHtmlString>> formatter)
        {
            Formatter = formatter;
            IsApplyable = isApplyable;
        }
    }

    public class EntityFormatterRule
    {
        public Func<HtmlHelper, Lite<IIdentifiable>, MvcHtmlString> Formatter { get; set; }
        public Func<Lite<IIdentifiable>, bool> IsApplyable { get; set; }

        public EntityFormatterRule(Func<Lite<IIdentifiable>, bool> isApplyable, Func<HtmlHelper, Lite<IIdentifiable>, MvcHtmlString> formatter)
        {
            Formatter = formatter;
            IsApplyable = isApplyable;
        }
    }

    public class CellFormatter
    {
        public bool WriteData;
        public Func<HtmlHelper, object, MvcHtmlString> Formatter;

        public MvcHtmlString WriteDataAttribute(object value)
        {
            if(!WriteData)
                return MvcHtmlString.Empty;

            string key = value is Lite<IdentifiableEntity> ? ((Lite<IdentifiableEntity>)value).Key() : value.TryToString();

            return MvcHtmlString.Create("data-value=" + key);
        }
    }
}
