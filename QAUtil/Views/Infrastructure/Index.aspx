<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Infrastructure
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        System</h2>
    <hr>
    <table border=0>
        <tr>
            <td>
                <li>Services</li>
                <ul id="submenu">
                    <li>
                        <%: Html.ActionLink("DEV", "Services", new { svcEnvironment = "DEV" }) %></li>
                    <li>
                        <%: Html.ActionLink("QA", "Services", new { svcEnvironment = "QA"}) %></li>
                    <li>
                        <%: Html.ActionLink("STAGING", "Services", new { svcEnvironment = "STAGING" }) %></li>
                </ul>
            </td>
            <td>
                <li>Application Pools</li>
                <ul id="submenu">
                    <li>
                        <%: Html.ActionLink("DEV", "ApplicationPools", new { appPoolEnvironment = "DEV" })%></li>
                    <li>
                        <%: Html.ActionLink("QA", "ApplicationPools", new { appPoolEnvironment = "QA" })%></li>
                    <li>
                        <%: Html.ActionLink("STAGING", "ApplicationPools", new { appPoolEnvironment = "STAGING" })%></li>
                </ul>
            </td>
        </tr>
    </table>
    <ul id="submenu">
        <li>
            <%: Html.ActionLink("Application Log", "ApplicationLog")%></li>
        <li>
            <%: Ajax.ActionLink("Modules", "Render", "Giving", new {submitValue = "Modules" }, new AjaxOptions() { UpdateTargetId = "ctrls" })%>
            <% using (Html.BeginForm("Modules", "Infrastructure")) { %>
            <div id="ctrls">
            </div>
            <% } %>
        </li>
        <li>
            <%: Html.ActionLink("Selenium Grid", "Selenium") %>
        </li>
        <li>Application Pools</li>
        <ul>
            <li>
                <%: Html.ActionLink("DEV", "ApplicationPools", new { appPoolEnvironment = "DEV" })%></li>
            <li>
                <%: Html.ActionLink("QA", "ApplicationPools", new { appPoolEnvironment = "QA" })%></li>
            <li>
                <%: Html.ActionLink("STAGING", "ApplicationPools", new { appPoolEnvironment = "STAGING" })%></li>
        </ul>
    </ul>
    <p>
</asp:Content>
