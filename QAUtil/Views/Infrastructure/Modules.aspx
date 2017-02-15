<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<QAUtil.Models.Modules>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Modules
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Modules</h2>
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
            <% using (Html.BeginForm("UpdateModules", "Infrastructure")) { %>
            <table>
                <tr>
                    <th>
                        Enabled
                    </th>
                    <th>
                        Module ID
                    </th>
                    <th>
                        Module Name
                    </th>
                    <th>
                        Created Date
                    </th>
                    <th>
                        Created By User ID
                    </th>
                    <th>
                        Last Updated Date
                    </th>
                    <th>
                        Last Updated By User ID
                    </th>
                    <th>
                        Created By Login
                    </th>
                    <th>
                        Last Updated By Login
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
                        <%: Html.TextBox("grid[" + i + "].MODULE_ID", item.MODULE_ID, new { @readonly = "readonly", @style = "width: 50px; border: 0px;" })%>
                    </td>
                    <td>
                        <%: item.MODULE_NAME %>
                    </td>
                    <td>
                        <%: String.Format("{0:g}", item.CREATED_DATE) %>
                    </td>
                    <td>
                        <%: item.CREATED_BY_USER_ID %>
                    </td>
                    <td>
                        <%: String.Format("{0:g}", item.LAST_UPDATED_DATE) %>
                    </td>
                    <td>
                        <%: item.LAST_UPDATED_BY_USER_ID %>
                    </td>
                    <td>
                        <%: item.CREATED_BY_LOGIN %>
                    </td>
                    <td>
                        <%: item.LAST_UPDATED_BY_LOGIN %>
                    </td>
                </tr>
                <% i++;
               } %>
            </table>
            <input type="submit" value="Update Modules">
            <% } %>
</asp:Content>
