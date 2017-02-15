<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<QAUtil.Models.ApplicationPool>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Application Pools
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Application Pools running in
        <%: ViewData["appPoolEnvironment"] %></h2>
    <hr>
    <table>
        <tr>
            <th>
                Action
            </th>
            <th>
                <%= Html.LabelFor(m => (m as QAUtil.Models.ApplicationPool).NAME) %>
            </th>
            <th>
                <%= Html.LabelFor(m => (m as QAUtil.Models.ApplicationPool).SERVER) %>
            </th>
        </tr>
        <% foreach (var item in Model) { %>
        <tr>
            <td>
                <%: Html.ActionLink("Recycle", "RecycleApplicationPool", new { appPoolEnvironment = ViewData["appPoolEnvironment"], server = item.SERVER, applicationPool = item.NAME }) %>
            </td>
            <td>
                <%: item.NAME %>
            </td>
            <td>
                <%: item.SERVER %>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>
