<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<label for="dataSource">
    Please select a data source.</label>
<select id="dataSource" name="dataSource">
    <option>DEV</option>
    <option>QA</option>
    <option>STAGING</option>
</select>
<label for="churchID">
    Church ID or Church Code</label>
<%: Html.TextBox("churchID", null, new {@style = "width: 100px;", @maxlength = 10})%>
<input type="submit" value=<%: ViewData["submitValue"] %>>