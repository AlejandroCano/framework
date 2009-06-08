<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="Signum.Entities.DynamicQuery" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Signum.Utilities" %>

<% FindOptions findOptions = (FindOptions)ViewData[ViewDataKeys.FindOptions];%>
<%=Html.Hidden(Html.GlobalName("sfQueryName"), findOptions.QueryName.ToString())%>
<%=Html.Hidden(Html.GlobalName("sfAllowMultiple"), findOptions.AllowMultiple.ToString())%>

<%= (findOptions.SearchOnLoad) ?
        "<script type=\"text/javascript\" language=\"javascript\">$(document).ready(function() {{ SearchOnLoad('{0}'); }});</script>".Formato(Html.GlobalName("btnSearch")) : 
    ""
%>

<div id="<%=Html.GlobalName("divFilters") %>" style="display:<%= (findOptions.FilterMode == FilterMode.Visible) ? "block" : "none" %>" >
<%Html.RenderPartial("~/Plugin/Signum.Web.dll/Signum.Web.Views.FilterBuilder.ascx", ViewData); %>
</div>
<% if (findOptions.FilterMode != FilterMode.AlwaysHidden){%>
    <input type="button" onclick="toggleVisibility('<%=Html.GlobalName("divFilters") %>');" value="Filtros" /> 
<%} %>
<input id="<%=Html.GlobalName("btnSearch")%>" type="button" onclick="<%="Search('/Signum/Search','{0}');".Formato(ViewData[ViewDataKeys.PopupPrefix] ?? "") %>" value="Buscar" /> 
<br />
<div id="<%=Html.GlobalName("divResults")%>" name="<%=Html.GlobalName("divResults")%>">

</div>