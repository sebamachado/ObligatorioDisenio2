<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Sitio.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content">
        <h2>Totales del sistema</h2>
        <asp:Label ID="lblError" runat="server" CssClass="message-error" />
        <div>
            <p>Total de usuarios: <asp:Label ID="lblTotalUsuarios" runat="server" /></p>
            <p>Mensajes activos: <asp:Label ID="lblMensajesActivos" runat="server" /></p>
            <p>Mensajes vencidos: <asp:Label ID="lblMensajesVencidos" runat="server" /></p>
            <p>Mensajes con más de cinco destinatarios: <asp:Label ID="lblMensajesMasCinco" runat="server" /></p>
        </div>
    </div>
</asp:Content>
