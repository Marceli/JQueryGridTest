<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<IEnumerable<Person>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Grid
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

   
    <h2>My Grid Data</h2>
    <p>
    <%=Html.ActionLink("Edit","Edit","Person") %>
    </p>
    <table id="list" class="scroll" cellpadding="0" cellspacing="0"></table>
    <div id="pager" class="scroll" style="text-align:center;"></div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ScriptHolder" runat="server">
<script type="text/javascript">
    $("pager").toggleClass
jQuery(document).ready(function(){ 
      jQuery("#list").jqGrid({
      url: './Person/GridData',
        datatype: 'json',
        mtype: 'POST',
        colNames: ['Id', 'FirstName', 'LastName'],
        colModel :[
          {name:'Id', index:'Id', width:100, align:'left' },
          { name: 'FirstName', index: 'Name.First', width: 200, align: 'left' },
          { name: 'LastName', index: 'Name.Last', width: 200, align: 'left'}],
          pager: jQuery('#pager'),
        rowNum:10,
        rowList: [10, 20, 30],
        sortname: 'Id',
        gridview:true,
        height: 250,
        altRows: true,
        sortorder: "desc",
        viewrecords: false,
        caption: 'My first grid'
      }); 
    }); 
    </script>
    
</asp:Content>
