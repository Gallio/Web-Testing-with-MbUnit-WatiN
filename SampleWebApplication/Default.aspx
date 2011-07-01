<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SampleWebApplication._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
  <script type="text/javascript" src="/Scripts/jquery-1.6.js"></script>
  <script type="text/javascript">
    function onButtonOk() {
      $("#LabelResultAsyncJQuery").load("/GetResultByJQueryAjax.aspx?name=" + $("#MainContent_TextBoxName").val())     
    }
  </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
<asp:ScriptManager ID="ScriptManager" runat="server"></asp:ScriptManager>
Please enter your name: <asp:TextBox ID="TextBoxName" runat="server"></asp:TextBox>
<fieldset>
  <legend>Synchronous Postback</legend>
  <asp:Button ID="ButtonOkSync" runat="server" Text="OK (Sync)" onclick="ButtonOkSync_Click" /><br />
  <asp:Label ID="LabelResultSync" runat="server" Text=""></asp:Label>
</fieldset>
<fieldset>
  <legend>Asynchronous Postback (with ASP.NET UpdatePanel)</legend>
  <asp:UpdatePanel ID="UpdatePanel" runat="server">
    <ContentTemplate>
      <asp:Button ID="ButtonOkAsync" runat="server" Text="OK (Async)" onclick="ButtonOkAsync_Click" /><br />
      <asp:Label ID="LabelResultAsync" runat="server" Text=""></asp:Label>
    </ContentTemplate>
  </asp:UpdatePanel>
</fieldset>
<fieldset>
  <legend>Asynchronous Postback (with jQuery AJAX functions)</legend>
  <input id="ButtonOkAsyncJQuery" type="button" value="OK (Async)" onclick="onButtonOk();" /><br />
  <span id="LabelResultAsyncJQuery"></span>
</fieldset>
</asp:Content>
