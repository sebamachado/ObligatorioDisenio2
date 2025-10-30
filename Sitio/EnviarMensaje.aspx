<%@ Page Title="Enviar Mensaje" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeBehind="EnviarMensaje.aspx.cs" Inherits="Sitio.EnviarMensaje" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content">
        <h2>Alta de mensaje</h2>
        <asp:Label ID="lblResultado" runat="server" />
        <div class="form-row">
            <label for="txtRemitente">Remitente</label>
            <asp:TextBox ID="txtRemitente" runat="server" MaxLength="8" />
        </div>
        <div class="form-row">
            <label for="ddlCategorias">Categoría</label>
            <asp:DropDownList ID="ddlCategorias" runat="server" DataTextField="Nombre" DataValueField="Codigo" />
        </div>
        <div class="form-row">
            <label for="txtAsunto">Asunto</label>
            <asp:TextBox ID="txtAsunto" runat="server" />
        </div>
        <div class="form-row">
            <label for="txtTexto">Texto</label>
            <asp:TextBox ID="txtTexto" runat="server" TextMode="MultiLine" Rows="4" />
        </div>
        <div class="form-row">
            <label for="txtCaducidad">Fecha de caducidad</label>
            <asp:TextBox ID="txtCaducidad" runat="server" placeholder="dd/MM/yyyy" />
        </div>
        <div class="form-row">
            <label for="txtDestinatarios">Destinatarios (uno por línea)</label>
            <asp:TextBox ID="txtDestinatarios" runat="server" TextMode="MultiLine" Rows="5" />
        </div>
        <div class="form-row">
            <asp:Button ID="btnEnviar" runat="server" Text="Enviar" OnClick="btnEnviar_Click" />
        </div>
    </div>
</asp:Content>
