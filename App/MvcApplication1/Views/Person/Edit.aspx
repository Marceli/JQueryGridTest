<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersonView>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Person
</asp:Content>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%Html.EnableClientValidation();%>
<%using(Html.BeginForm("Update","Person")){%>
<%=Html.ClientValidationEnabled.ToString()%>


    <h2>Edit</h2>
    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>
    <br />
  <%=Html.EditorFor(m=>m) %>
  <br />
<input type="submit" />
<%}%>
</asp:Content>
