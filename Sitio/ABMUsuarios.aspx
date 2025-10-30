<%@ Page Title="Usuarios" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeBehind="ABMUsuarios.aspx.cs" Inherits="Sitio.ABMUsuarios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="content">
        <h2>Administración de usuarios</h2>
        <asp:Label ID="lblMensaje" runat="server" />
        <div class="form-row">
            <label for="txtUsername">Usuario</label>
            <asp:TextBox ID="txtUsername" runat="server" MaxLength="8" />
        </div>
        <div class="form-row">
            <label for="txtPassword">Contraseña</label>
            <asp:TextBox ID="txtPassword" runat="server" MaxLength="8" />
        </div>
        <div class="form-row">
            <label for="txtNombre">Nombre completo</label>
            <asp:TextBox ID="txtNombre" runat="server" />
        </div>
        <div class="form-row">
            <label for="txtFecha">Fecha de nacimiento</label>
            <asp:TextBox ID="txtFecha" runat="server" placeholder="dd/MM/yyyy" />
        </div>
        <div class="form-row">
            <label for="txtEmail">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" />
        </div>
        <div class="form-row">
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar" OnClick="btnBuscar_Click" />
            <asp:Button ID="btnCrear" runat="server" Text="Crear" OnClick="btnCrear_Click" />
            <asp:Button ID="btnModificar" runat="server" Text="Modificar" OnClick="btnModificar_Click" />
            <asp:Button ID="btnPassword" runat="server" Text="Actualizar contraseña" OnClick="btnPassword_Click" />
            <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" OnClick="btnEliminar_Click" />
        </div>
        <h3>Usuarios registrados</h3>
        <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="false" CssClass="table">
            <Columns>
                <asp:BoundField DataField="Username" HeaderText="Usuario" />
                <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="FechaNacimiento" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Nacimiento" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
