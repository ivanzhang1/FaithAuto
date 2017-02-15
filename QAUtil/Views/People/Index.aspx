<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Insert Communication Value</h2>
    <h3>
        <%= ViewData["Message"] %></h3>
    <hr/>
    <% using (Html.BeginForm("InsertCommValue", "People")) { %>
    <% Html.RenderPartial("../Shared/DataSourceUserControl"); %>
    <label for="churchID">
        Church ID i.e. 15</label>
    <%: Html.TextBox("churchID", null, new { @style = "width: 50px;", @maxlength = 6 }) %>
    <br />
    <label for="indID">Individual ID</label>
    <%: Html.TextBox("indID", null, new { @style = "width: 50px;", @maxlength = 18 }) %>
    <br />
    <label for="hsdID">Household ID</label>
    <%: Html.TextBox("hsdID", null, new { @style = "width: 50px;", @maxlength = 18 }) %>
    <br />
    <label for="commType">Communication Type</label>
    <%: Html.TextBox("commType", null, new { @style = "width: 50px;", @maxlength = 18 }) %>
    <br />
    <label for="commValue">Communication Value</label>
    <%: Html.TextBox("commValue", null, new { @style = "width: 50px;", @maxlength = 18 }) %>
    <br />
    <input type="submit" value="Insert Comm Value"/>
    <% } %>
</asp:Content>
