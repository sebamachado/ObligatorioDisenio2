<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h2>Resumen general</h2>
    <asp:Label ID="lblMensaje" runat="server" CssClass="validation" />
    <table class="table">
        <tr>
            <th>Total de usuarios</th>
            <td><asp:Label ID="lblTotalUsuarios" runat="server" /></td>
        </tr>
        <tr>
            <th>Total de mensajes</th>
            <td><asp:Label ID="lblTotalMensajes" runat="server" /></td>
        </tr>
        <tr>
            <th>Mensajes con más de cinco destinatarios</th>
            <td><asp:Label ID="lblMasDestinatarios" runat="server" /></td>
        </tr>
    </table>
</asp:Content>
