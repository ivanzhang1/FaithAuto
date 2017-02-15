<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<br>
            <label for="accountNumber">Account #</label>
            <%: Html.TextBox("accountNumber", 6784648310, new { @maxlength = 12 }) %>
            <label for="routingNumber">Routing #</label>
            <%: Html.TextBox("routingNumber", 111900659, new { @maxlength = 12 }) %>