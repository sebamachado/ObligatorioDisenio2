<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MP.master" CodeFile="ListarFacturas.aspx.cs" Inherits="ListarFacturas" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>

            <br />
    <asp:Button ID="BtnFiltrar" runat="server"  Text="Filtrar" OnClick="BtnFiltrar_Click" />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:Button ID="BtnSinFiltro" runat="server"  Text="Limpiar" OnClick="BtnSinFiltro_Click" />
        <p> Facturas de un Usuario: 
                <asp:DropDownList ID="DDLUsuarios" runat="server" Height="27px" Width="188px">
                </asp:DropDownList>
&nbsp;
                </p>
    <p> Facturas de una Fecha:
        <asp:TextBox ID="TxtFecha" runat="server" TextMode="Date" Width="180px"></asp:TextBox>
&nbsp;</p>
    <p> Factruas del Articulo 
                <asp:DropDownList ID="DDLArticulos" runat="server" Height="27px" Width="188px">
                </asp:DropDownList>
    </p>
        <p> &nbsp;</p>
        <p> Estadisticas:
            <asp:Button ID="BtnEstad1" runat="server" OnClick="BtnEstad1_Click" Text="Ventas por mes/Año" />
&nbsp;
            <asp:Button ID="BtnEstad2" runat="server" Text="Listado Articulos" OnClick="BtnEstad2_Click" />
&nbsp;
            <asp:Button ID="BtnEstad3" runat="server" Text="Listado Vendedores" OnClick="BtnEstad3_Click" />
    </p>
        <p> 
            <asp:GridView ID="GvEstadisticas" runat="server" Width="680px">
            </asp:GridView>
    </p>


    <div class="auto-style11">
    
        <strong>Las Facturas del Sistema
        
        </strong></div>
        
        <asp:GridView ID="gvFacturas" runat="server" Height="145px" Width="702px" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="dgvFacturas_PageIndexChanging" OnSelectedIndexChanged="gvFacturas_SelectedIndexChanged" >
            <Columns>
                <asp:BoundField DataField="numFact" HeaderText="Numero" />
                <asp:BoundField DataField="FechaFact" HeaderText="Fecha" />
                <asp:BoundField DataField="Usuarios.UsuLog" HeaderText="Usuario" />
                <asp:CommandField SelectText="Ver Lineas" ShowSelectButton="True" />
            </Columns>
        </asp:GridView>
        <br />
        <br />
        <br />
         <div class="auto-style12">
    
             <strong>Las lineas de la Factura Seleccionada</strong></div>
        
        <asp:GridView ID="gvLineas" runat="server" Height="145px" Width="416px" AutoGenerateColumns="False">
            <Columns>
                 <asp:BoundField DataField="Articulos.CodArt" HeaderText="Articulo" />
               <asp:BoundField DataField="Articulos.NomArt" HeaderText="Nombre" />
                <asp:BoundField DataField="Articulos.PreArt" HeaderText="Precio" />
                <asp:BoundField DataField="Cant" HeaderText="Cantidad" />
            </Columns>
        </asp:GridView>
</asp:Content>
<asp:Content ID="Content3" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .auto-style11 {
            width: 414px;
            text-decoration: underline;
            color: #00FFFF;
            font-size: xx-large;
        }
    .auto-style12 {
        width: 589px;
        text-decoration: underline;
        color: #00FFFF;
        font-size: xx-large;
    }
    </style>
</asp:Content>
