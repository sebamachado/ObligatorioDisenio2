<%@ Page Title="Bandeja" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="Bandeja.aspx.cs" Inherits="Bandeja" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h2><asp:Label ID="lblTitulo" runat="server" /></h2>
    <asp:Label ID="lblMensaje" runat="server" CssClass="validation" />
    <asp:DropDownList ID="ddlUsuario" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlUsuario_SelectedIndexChanged" />
    <asp:GridView ID="gvMensajes" runat="server" AutoGenerateColumns="false" CssClass="table">
        <Columns>
            <asp:BoundField DataField="FechaEnvio" HeaderText="Fecha" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
            <asp:BoundField DataField="Asunto" HeaderText="Asunto" />
            <asp:BoundField DataField="Categoria.Nombre" HeaderText="Categoría" />
            <asp:BoundField DataField="Remitente.Username" HeaderText="Remitente" />
            <asp:TemplateField HeaderText="Destinatarios">
                <ItemTemplate>
                    <asp:Repeater ID="repDestinatarios" runat="server" DataSource='<%# ((ModeloEF.Mensajes)Container.DataItem).Destinatarios %>'>
                        <ItemTemplate>
                            <%# Eval("Username") %><br />
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
