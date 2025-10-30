<%@ Page Title="Bandeja" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeBehind="Bandeja.aspx.cs" Inherits="Sitio.Bandeja" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content">
        <h2>Bandeja de mensajes</h2>
        <asp:Label ID="lblInfo" runat="server" />
        <div class="form-row">
            <label for="txtUsuario">Usuario</label>
            <asp:TextBox ID="txtUsuario" runat="server" MaxLength="8" />
            <asp:Button ID="btnBandeja" runat="server" Text="Ver bandeja" OnClick="btnBandeja_Click" />
            <asp:Button ID="btnEnviados" runat="server" Text="Ver enviados" OnClick="btnEnviados_Click" />
        </div>
        <asp:GridView ID="gvMensajes" runat="server" AutoGenerateColumns="false" CssClass="table">
            <Columns>
                <asp:BoundField DataField="Id" HeaderText="Id" />
                <asp:BoundField DataField="Asunto" HeaderText="Asunto" />
                <asp:BoundField DataField="Remitente" HeaderText="Remitente" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:BoundField DataField="FechaEnvio" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderText="Envío" />
                <asp:BoundField DataField="FechaCaducidad" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Caducidad" />
                <asp:BoundField DataField="Destinatarios" HeaderText="Destinatarios" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
