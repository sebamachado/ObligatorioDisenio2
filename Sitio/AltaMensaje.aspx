<%@ Page Title="Nuevo mensaje" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="AltaMensaje.aspx.cs" Inherits="AltaMensaje" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h2>Alta de mensaje</h2>
    <asp:Label ID="lblMensaje" runat="server" CssClass="validation" />
    <table class="table">
        <tr>
            <th>Remitente</th>
            <td><asp:DropDownList ID="ddlRemitente" runat="server" /></td>
        </tr>
        <tr>
            <th>Categoría</th>
            <td><asp:DropDownList ID="ddlCategoria" runat="server" /></td>
        </tr>
        <tr>
            <th>Asunto</th>
            <td><asp:TextBox ID="txtAsunto" runat="server" MaxLength="50" /></td>
        </tr>
        <tr>
            <th>Texto</th>
            <td><asp:TextBox ID="txtTexto" runat="server" TextMode="MultiLine" Rows="4" MaxLength="100" /></td>
        </tr>
        <tr>
            <th>Fecha de caducidad</th>
            <td><asp:TextBox ID="txtFechaCaducidad" runat="server" TextMode="Date" /></td>
        </tr>
        <tr>
            <th>Destinatarios</th>
            <td><asp:ListBox ID="lstDestinatarios" runat="server" SelectionMode="Multiple" Rows="6" /></td>
        </tr>
    </table>
    <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" />
</asp:Content>
