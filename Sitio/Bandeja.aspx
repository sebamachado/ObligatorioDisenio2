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
                <asp:BoundField DataField="RemitenteUsername" HeaderText="Remitente" />
                <asp:TemplateField HeaderText="Categoría">
                    <ItemTemplate><%# Eval("Categoria.Nombre") %></ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaEnvio" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderText="Envío" />
                <asp:BoundField DataField="FechaCaducidad" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Caducidad" />
                <asp:TemplateField HeaderText="Destinatarios">
                    <ItemTemplate>
                        <%# Eval("Destinatarios") == null ? string.Empty : string.Join(", ", System.Linq.Enumerable.Select((System.Collections.Generic.IEnumerable<EntidadesCompartidas.Entidades.Usuario>)Eval("Destinatarios"), u => u.Username.Trim())) %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
