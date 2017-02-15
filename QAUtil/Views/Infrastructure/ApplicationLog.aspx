<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	InsertApplicationLogRecordForRDC
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Insert Application Log Record For RDC</h2>
    <hr>
    <% using (Html.BeginForm("InsertApplicationLogRecordForRDC", "Infrastructure")) { %>
        <% Html.RenderPartial("../Shared/DataSourceUserControl"); %>
        <input type="submit" value="Insert Starting Record">
	<% } %>
    <br>
    <% using (Html.BeginForm("DeleteApplicationLogsForRDC", "Infrastructure")) { %>
        <% Html.RenderPartial("../Shared/DataSourceUserControl"); %>
        <input type="submit" value="Delete Application Logs">
	<% } %>

</asp:Content>
