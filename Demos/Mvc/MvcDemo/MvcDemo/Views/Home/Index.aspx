<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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
