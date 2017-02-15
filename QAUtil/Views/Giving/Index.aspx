<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Giving
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Giving</h2>
    <hr>
    <ul id="submenu">
        <li>
            <%: Html.ActionLink("Delete Batches", "BatchesDelete", "Giving", new { batchTypeID = "1" }, null) %></li>
        <li>
            <%: Html.ActionLink("Delete RDC Batches", "BatchesDelete", "Giving", new { batchTypeID = "3" }, null) %></li>
        <li>
            <%: Html.ActionLink("Mock Up an RDC Batch", "Batches_RemoteDepositCapture") %></li>
        <li>
            <%: Ajax.ActionLink("Payments", "Render", "Giving", new {submitValue = "Payments" }, new AjaxOptions() { UpdateTargetId = "ctrls" })%>
            <% using (Html.BeginForm("Payments", "Giving")) { %>
            <div id="ctrls">
            </div>
            <% } %>
        </li>
        <li>
            <%: Ajax.ActionLink("Payment Types", "Render", "Giving", new {submitValue = "PaymentTypes" }, new AjaxOptions() { UpdateTargetId = "ctrls2" })%>
            <% using (Html.BeginForm("PaymentTypes", "Giving")) { %>
            <div id="ctrls2">
            </div>
            <%--<select id="feature" name="feature">
                <option>Online Giving 1.0</option>
                <option>Online Giving 2.0</option>
            </select>--%>
            <% } %>
        </li>
    </ul>
</asp:Content>
