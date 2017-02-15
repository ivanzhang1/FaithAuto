<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<QAUtil.Models.Payments>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Payments
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Payments</h2>
    <table>
        <tr>
            <th>
                <%: Html.LabelFor(m => Model.ElementAt(0).CreatedDate) %>
            </th>
            <th>
                <%: Html.LabelFor(m => Model.ElementAt(0).PaymentStatusName) %>
            </th>
            <th>
                HouseholdID
            </th>
            <th>
                IndividualID
            </th>
            <th>
                Amount
            </th>
            <th>
                ReasonCode
            </th>
            <th>
                Description
            </th>
            <th>
                <%: Html.LabelFor(m => Model.ElementAt(0).ClientApplication) %>
            </th>
        </tr>
        <% foreach (var item in Model) { %>
        <tr>
            <td>
                <%: item.CreatedDate %>
            </td>
            <td>
                <%: item.PaymentStatusName %>
            </td>
            <td>
                <%: item.HouseholdID %>
            </td>
            <td>
                <%: item.IndividualID %>
            </td>
            <td>
                <%: item.Amount %>
            </td>
            <td>
                <%: item.ReasonCode %>
            </td>
            <td>
                <%: item.Description %>
            </td>
            <td>
                <%: item.ClientApplication %>
            </td>
        </tr>
        <% } %>
    </table>
</asp:Content>
