<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<QAUtil.Models.TestSuite>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Tests
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Tests
    </h2>
    <table>
        <tr>
            <th>
                <%= Html.LabelFor(m => (m as QAUtil.Models.TestSuite).Name) %>
            </th>
            <th>
                <%= Html.LabelFor(m => (m as QAUtil.Models.TestSuite).Duration) %>
            </th>
            <th>
                <%= Html.LabelFor(m => (m as QAUtil.Models.TestSuite).TestCases) %>
            </th>
        </tr>
        <% foreach (var item in Model) { %>
        <tr>
            <td>
                <%: item.Name %>
            </td>
            <td>
                <%: item.Duration %>
            </td>
            <td>
                <%: item.TestCases %>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>
