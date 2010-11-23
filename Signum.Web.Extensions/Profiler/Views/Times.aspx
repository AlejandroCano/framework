<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Signum.Web" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="Signum.Utilities" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<style type="text/css">
.task {
	border-bottom:1px solid #ddd;
}
.task .processName{
	font-size:150%;	
}

.task .numTimes{
	color:#666;
}
.task .max, .task .med, .task .min, .task .last{
	display:block;	
	height:10px;
	float:left;
	margin-right:10px;
	margin-left:0px;
}

.task .max{
	background: #b22222 url(../images/alert-overlay.png) repeat-x;
}
.task .med{
	background: #ff7722 url(../images/alert-overlay.png) repeat-x;
}
.task .min{
	background: #ffd700 url(../images/alert-overlay.png) repeat-x;
}
.task .last{
	background: #b27aff url(../images/alert-overlay.png) repeat-x;
}
.leftBorder{
	border-left:1px solid #eee;
}
#tasks{
	width:1200px;	
}

#tasks .left{float: left;}


.tblResults .percentile0{
	background-color: #99CCFF;
}
.tblResults .percentile1{
	background-color: #fff;
}
.tblResults .percentile2{
	background-color: #FFCCCC;
}
</style>
<%=Html.ActionLink("Clear", "ClearTimes")%>
<ul id="tasks">
        <%
            
            int maxWitdh = 600;
            long maxValue = maxWitdh;
            if (TimeTracker.IdentifiedElapseds.Count > 0)
                maxValue = TimeTracker.IdentifiedElapseds.OrderByDescending(p => p.Value.MaxTime).First().Value.MaxTime;

            double ratio = 1;
            if (maxValue != 0) ratio = maxWitdh / (double)maxValue;
            foreach (KeyValuePair<string, TimeTrackerEntry> pair in TimeTracker.IdentifiedElapseds.OrderByDescending(p => p.Value.Average))
            {    
         %>
    <li class="task">
        <table>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td width="300">
                                <span class="processName"><%= pair.Key.Split(' ')[0] %></span>
                                <% if (pair.Key.Split(' ').Length > 1)
                                   { %>
                                <br />
                                <span class="entityName"><%= pair.Key.Split(' ')[1]%></span>
                                <%}%>
                                
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <span class="numTimes">Executed <%= pair.Value.Count %> <%= pair.Value.Count == 1 ? "time" : "times"%></span>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table>
                        <% if (pair.Value.Count == 1)
                           {%>
                            <tr>
                            <td width="40">
                                Single:
                            </td>
                            <td class="leftBorder">
                                <span class="med" style="width:<%= (int) (pair.Value.Average * ratio) %>px"></span> <%= pair.Value.LastTime %> ms (hace <%=new TimeSpan(DateTime.UtcNow.Subtract(pair.Value.LastDate).Ticks).ToShortString()%>)
                            </td>
                        </tr>
                        <% }
                           else
                           { %>
                        <tr>
                            <td width="40">
                                Max
                            </td>
                            <td class="leftBorder">
                                <span class="max" style="width:<%= (int) (pair.Value.MaxTime * ratio) %>px"></span><%= pair.Value.MaxTime%> ms (hace <%=new TimeSpan(DateTime.UtcNow.Subtract(pair.Value.MaxDate).Ticks).ToShortString()%>)
                            </td>
                        </tr>
                        <tr>
                            <td width="40">
                                Average
                            </td>
                            <td class="leftBorder">
                                <span class="med" style="width:<%= (int) (pair.Value.Average * ratio) %>px"></span><%= pair.Value.Average%> ms
                            </td>
                        </tr>
                        <tr>
                            <td width="40">
                                Min
                            </td>
                            <td class="leftBorder">
                                <span class="min" style="width:<%= (int) (pair.Value.MinTime * ratio) %>px"></span><%= pair.Value.MinTime%> ms (hace <%= new TimeSpan(DateTime.UtcNow.Subtract(pair.Value.MinDate).Ticks).ToShortString()%>)
                            </td>
                        </tr>
                          <tr>
                            <td width="40">
                                Last
                            </td>
                            <td class="leftBorder">
                                <span class="last" style="width:<%= (int) (pair.Value.LastTime * ratio) %>px"></span><%= pair.Value.LastTime%> ms (hace <%= new TimeSpan(DateTime.UtcNow.Subtract(pair.Value.LastDate).Ticks).ToShortString()%>)
                            </td>
                        </tr> 
                        <% } %>                                               
                    </table>
                </td>
            </tr>
        </table>
    </li>
    <% } %>
</ul>
</asp:Content>
