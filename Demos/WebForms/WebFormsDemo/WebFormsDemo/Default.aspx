<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebFormsDemo._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h1>Css</h1>
    <p>This code in the master page:</p>
    <p>
        <code><%= HttpUtility.HtmlEncode("<%= Rejuiced.CssFor(\"~/Combined.css\")") %></code>
    </p>
    <p>produces</p>
    <p>
        <code><%= HttpUtility.HtmlEncode(Rejuiced.CssFor("~/Combined.css").ToString()) %></code>
    </p>

    <div class="testImageCssTransform"></div>
    <h1>Js</h1>
    <p>This code in the master page:</p>
    <p>
        <code><%= HttpUtility.HtmlEncode("<%= Rejuiced.JsFor(\"~/Combined-{0}.js\")")%></code>
    </p>
    <p>produces</p>
    <p>
        <code><%= HttpUtility.HtmlEncode(Rejuiced.JsFor("~/Combined-{0}.js").ToString()) %></code>
    </p>
</asp:Content>
