<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<QAUtil.Models.PaymentTypes>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PaymentTypes
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        PaymentTypes</h2>
    <p>
        <label for="dataSource">
            Environment:
        </label>
        <b>
            <%: Html.Label(ViewData["dataSource"].ToString()) %></b>
        <label for="churchId">
            Church ID:</label>
        <b>
            <%: Html.Label(ViewData["churchId"].ToString()) %>
            <hr>
            <% using (Html.BeginForm("UpdatePaymentTypes", "Giving")) { %>
            <table>
                <tr>
                    <th>
                        Enabled
                    </th>
                    <th>
                        Payment Type ID
                    </th>
                    <th>
                        Payment Type
                    </th>
                    <th>
                        Feature ID
                    </th>
                </tr>
                <%: Html.Hidden("church", ViewData["churchId"]) %>
                <%: Html.Hidden("dataSource", ViewData["dataSource"]) %>
                <% int i = 0;
                   foreach (var item in Model) { %>
                <tr>
                    <td>
                        <%: Html.CheckBox("grid[" + i + "].ENABLED", item.ENABLED) %>
                    </td>
                    <td>
                        <%: Html.TextBox("grid[" + i + "].PP_TYPE_ID", item.PP_TYPE_ID, new { @readonly = "readonly", @style = "width: 50px; border: 0px;" })%>
                    </td>
                    <td>
                        <%: item.PaymentType %>
                    </td>
                    <td>
                        <%: Html.TextBox("grid[" + i + "].FeatureID", item.FeatureID) %>
                    </td>
                </tr>
                <% i++;
                   } %>
            </table>
            <input type="submit" value="Update Payment Types">
            <% } %>
</asp:Content>
