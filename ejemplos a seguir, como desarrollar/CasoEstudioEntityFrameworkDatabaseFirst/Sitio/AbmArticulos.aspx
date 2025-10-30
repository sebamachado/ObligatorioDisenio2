<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MP.master" CodeFile="AbmArticulos.aspx.cs" Inherits="AbmArticulos" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <table border="1">
        <tr>
            <td colspan="3" class="style1"><strong>ABM de Articulos </strong></td>
        </tr>    
        <tr>
            <td> Codigo: </td>
            <td><asp:TextBox ID="txtCodigo" runat="server"></asp:TextBox></td>
            <td><asp:Button ID="BtnBuscar" runat="server" Text="Buscar" OnClick="BtnBuscar_Click" 
                     /></td>
        </tr>
        <tr>
            <td> Nombre: </td>
            <td>  <asp:TextBox ID="txtNombre" runat="server"></asp:TextBox> </td>
            <td> &nbsp;</td>
        </tr>
        <tr>
            <td> Precio: </td>
            <td> <asp:TextBox ID="txtPrecio" runat="server"></asp:TextBox> </td>
            <td>  &nbsp; </td>        
        </tr>
        <tr>
            <td><asp:Button ID="BtnAlta" runat="server" Text="Alta" Enabled="False" OnClick="BtnAlta_Click" 
                    /></td>
            <td><asp:Button ID="BtnBaja" runat="server" Text="Baja" Enabled="False" OnClick="BtnBaja_Click"  /></td>
            <td><asp:Button ID="BtnModificar" runat="server" Text="Modificar" Enabled="False" OnClick="BtnModificar_Click"  /></td>
        </tr>
        <tr>
            <td colspan="3">  &nbsp;  
                <asp:Button ID="BtnLimpiar" runat="server" OnClick="BtnLimpiar_Click" Text="Limpiar" />
            </td>
        </tr>
        <tr>
            <td colspan="3"> <asp:Label ID="lblError" runat="server" ForeColor="Red" Width="386px"></asp:Label> </td>
        </tr>
        <tr>
            <td colspan="3">  &nbsp; </td>
        </tr>
        <tr>
            <td colspan="3"><asp:Button ID="BtnListar" runat="server" 
                    Text="Listar Todos los Articulos" OnClick="BtnListar_Click"  /></td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:GridView ID="gvListado" runat="server" AutoGenerateColumns="False" Height="197px"  Width="456px">
                  <Columns>
                        <asp:BoundField DataField="CodArt" HeaderText="Codigo" />
                        <asp:BoundField DataField="NomArt" HeaderText="Nombre" />
                        <asp:BoundField DataField="PreArt" HeaderText="Precio" />
                  </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>