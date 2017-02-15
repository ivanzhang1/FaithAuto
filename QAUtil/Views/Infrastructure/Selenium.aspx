<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Selenium
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="refresh" content="30" />
    <table>
        <tr>
            <td valign="top">
                <%= ViewData["Environments"] %>
            </td>
            <td valign="top">
                <%= ViewData["rcAvail"] %>
            </td>
            <td valign="top">
                <%= ViewData["rcActive"] %>
            </td>
        </tr>
    </table>
    <% using (Html.BeginForm("UnregisterRemoteControl", "System")) { %>
    <%: Html.TextBox("host", "Host name") %>
    <%: Html.TextBox("port", "Port number") %>
    <input type="submit" value="Unregister Remote Control">
    <%} %>
</asp:Content>
