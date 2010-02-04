<%@ Control Language="C#" Inherits="ModelViewUserControl<Person>" %>
    <h2>Edit</h2>
    <br />
  <%=this.Hidden(p=>p.Id) %>
  <%=this.TextBox(p => p.FirstName).Size(20).Label("First Name:").Title("This is my title").Class("fieldset")%>
  <%=this.ValidationMessage(x=>x.FirstName, "Please enter a valid date")%><br />
   <br />
  <%=this.TextBox(p => p.LastName).Size(20).Label("Last Name:").Title("This is my title").Class("fieldset")%>
  <br />
<input type="submit" />

