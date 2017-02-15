<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<QAUtil.Models.Services>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Services
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Services running in
        <%: ViewData["svcEnvironment"] %></h2>
    <hr>
    <table>
        <tr>
            <th>
                Action
            </th>
            <th>
                Service
            </th>
            <th>
                Server
            </th>
            <th>
                Status
            </th>
            <th>
                State
            </th>
            <th>
                Account
            </th>
            <th>
                Description
            </th>
        </tr>
        <% foreach (var item in Model) { %>
        <tr>
            <td>
                <% string cmd = item.STATE == "Running" ? "Stop" : "Start"; %>
                <%: Html.ActionLink(cmd, "ToggleService", new { svcEnvironment = ViewData["svcEnvironment"], server = item.SERVER, service = item.NAME, command = cmd }) %>
            </td>
            <td>
                <%: item.NAME %>
            </td>
            <td>
                <%: item.SERVER %>
            </td>
            <td>
                <%: item.STATUS %>
            </td>
            <td>
                <%: item.STATE %>
            </td>
            <td>
                <%: item.STARTNAME %>
            </td>
            <td>
                <%: item.DESCRIPTION %>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>
