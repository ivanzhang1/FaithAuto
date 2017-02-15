<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	MockUpRDCBatch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>MockUpRDCBatch</h2>
    <hr>
    <% using (Html.BeginForm("MockUpRDCBatch", "Giving")) { %>
        <% Html.RenderPartial("../Shared/DataSourceUserControl"); %>
        <label for="churchId">Church ID i.e. 15</label>
        <%: Html.TextBox("churchId", 15, new { @style = "width: 50px;", @maxlength = 4 }) %>
        <label for="batchName">Batch Name:</label>
        <%: Html.TextBox("batchName", null, new { @style = "width: 200px;" }) %>
        <label for="batchAmount">Batch Item Amount:</label>
        <%: Html.TextBox("batchItemAmount", null, new { @style = "width: 50px;" }) %>
        <label for="batchItemCount">Batch Item Count:</label>
        <%: Html.TextBox("batchItemCount", null, new { @style = "width: 50px;" }) %>
        <input type="submit" value="Create RDC Batch">
    <% } %>

</asp:Content>
