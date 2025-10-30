<%@ Page Title="Gestión de usuarios" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="ABMUsuarios.aspx.cs" Inherits="ABMUsuarios" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContenidoPrincipal" runat="server">
    <h2>Usuarios</h2>
    <asp:Label ID="lblMensaje" runat="server" CssClass="validation" />
    <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="false" CssClass="table" DataKeyNames="Username" OnSelectedIndexChanged="gvUsuarios_SelectedIndexChanged">
        <Columns>
            <asp:BoundField DataField="Username" HeaderText="Usuario" />
            <asp:BoundField DataField="NombreCompleto" HeaderText="Nombre" />
            <asp:BoundField DataField="Email" HeaderText="Correo" />
            <asp:BoundField DataField="FechaNacimiento" HeaderText="Nacimiento" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:CommandField ShowSelectButton="true" SelectText="Editar" />
        </Columns>
    </asp:GridView>

    <h3>Datos</h3>
    <table class="table">
        <tr>
            <th>Usuario</th>
            <td><asp:TextBox ID="txtUsuario" runat="server" MaxLength="8" /></td>
        </tr>
        <tr>
            <th>Contraseña</th>
            <td><asp:TextBox ID="txtPassword" runat="server" MaxLength="8" TextMode="Password" /></td>
        </tr>
        <tr>
            <th>Nombre completo</th>
            <td><asp:TextBox ID="txtNombre" runat="server" MaxLength="50" /></td>
        </tr>
        <tr>
            <th>Correo electrónico</th>
            <td><asp:TextBox ID="txtEmail" runat="server" MaxLength="100" /></td>
        </tr>
        <tr>
            <th>Fecha de nacimiento</th>
            <td><asp:TextBox ID="txtFechaNacimiento" runat="server" TextMode="Date" /></td>
        </tr>
    </table>
    <asp:Button ID="btnNuevo" runat="server" Text="Nuevo" OnClick="btnNuevo_Click" />
    <asp:Button ID="btnGuardar" runat="server" Text="Guardar" OnClick="btnGuardar_Click" />
    <asp:Button ID="btnEliminar" runat="server" Text="Eliminar" OnClick="btnEliminar_Click" />
</asp:Content>
