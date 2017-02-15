<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Delete Batches
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Delete Batches</h2>
    <h3>
        <%= ViewData["Message"] %></h3>
    <hr>
    <% using (Html.BeginForm("DeleteBatches", "Giving")) { %>
    <% Html.RenderPartial("../Shared/DataSourceUserControl"); %>
    <label for="churchID">
        Church ID i.e. 15</label>
    <%: Html.TextBox("churchID", null, new { @style = "width: 50px;", @maxlength = 4 }) %>
    <label for="fromDate">
        Starting date in MM/DD/YYYY format</label>
    <%: Html.TextBox("fromDate", null, new { @style = "width: 50px;", @maxlength = 10 }) %>
    <%: Html.Hidden("batchTypeID", ViewData["batchTypeID"]) %>
    <% if (Convert.ToInt16(ViewData["batchTypeID"]) == 3) { %>
    <hr>
    <br>
    <label for="deleteAccounts">
        Delete accounts tied to testing account?</label>
    <%: Html.CheckBox("deleteAccounts", false) %>
    <%: Html.TextBox("accountNumber", "9600137549", new { @hidden = true }) %>
    <%: Html.TextBox("routingNumber", "041203824", new { @hidden = true }) %>
    <% } %>
    <input type="submit" value="Delete Batches">
    <% } %>
</asp:Content>
