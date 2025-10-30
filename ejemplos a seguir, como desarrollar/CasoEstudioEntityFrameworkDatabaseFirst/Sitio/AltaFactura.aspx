<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MP.master" CodeFile="AltaFactura.aspx.cs" Inherits="AltaFactura" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


    <div class="style3">
    
        <h2>
    
        <span class="style2">Alta Facturas</span></h2>
        <br />
        <table align="center" style="width: 48%;">
            <tr>
                <td class="style10">
                    Numero:</td>
                <td class="style5">
        <asp:TextBox ID="txtNro" runat="server" TextMode="Number"></asp:TextBox>
                </td>
                <td class="style6">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style10">
                    Fecha:</td>
                <td class="style5">
        <asp:TextBox ID="txtFecha" runat="server" TextMode="Date"></asp:TextBox>
                </td>
                <td class="style6">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style10">
                    Articulo:</td>
                <td class="style5">
                    <asp:TextBox ID="txtCodigoArticulo" runat="server" TextMode="Number"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                <td class="style6">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style10">
                    cantidad</td>
                <td class="style5">
                    <asp:TextBox ID="txtCantidad" runat="server" Width="52px" TextMode="Number"></asp:TextBox>
                </td>
                <td class="style6">
                    <asp:Button ID="btnAgregarArticulo" runat="server" 
                         Text="Añadir articulo a la lista" 
                        Width="194px" OnClick="btnAgregarArticulo_Click" />
                </td>
            </tr>
            <tr>
                <td class="style9" colspan="3">
                    <br />
                    Lineas de la Factura<br />
                </td>
            </tr>
            <tr>
                <td class="style4" colspan="3">
                    <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="False" Width="248px">
                        <Columns>
                            <asp:BoundField DataField="Articulos.CodArt" HeaderText="Codigo" />
                             <asp:BoundField DataField="Articulos.NomArt" HeaderText="Nombre" />
                           <asp:BoundField DataField="Cant" HeaderText="Cantidad" />
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td class="style4" colspan="3">
                     <asp:Button ID="btnAgregar" runat="server"  
            Text="Agregar Factura" Enabled="False" OnClick="btnAgregar_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:Button ID="BtnLimpiar" runat="server" OnClick="BtnLimpiar_Click" Text="Limpiar" />
            </table>
        <br />
        <asp:Label ID="lblError" runat="server" ForeColor="Red" Width="386px"></asp:Label>
        <br />
        <br />
    
    </div>
 </asp:Content>
  <asp:Content ID="Content3" runat="server" contentplaceholderid="head">
      <style type="text/css">
          .style4 {
              text-align: center;
          }
      </style>
</asp:Content>

  