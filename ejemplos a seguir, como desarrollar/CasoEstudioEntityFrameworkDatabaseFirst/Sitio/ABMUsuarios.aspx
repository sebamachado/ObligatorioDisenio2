<%@ Page Title="" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="ABMUsuarios.aspx.cs" Inherits="ABMUsuarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .auto-style11 {
            width: 43%;
        }
        .auto-style12 {
            font-size: x-large;
        }
        .auto-style13 {
            width: 185px;
        }
        .auto-style14 {
            width: 265px;
        }
        .auto-style15 {
            width: 185px;
            height: 25px;
        }
        .auto-style16 {
            width: 265px;
            height: 25px;
        }
        .auto-style17 {
            height: 25px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table border="1" class="auto-style11">
        <tr>
            <td class="auto-style12" colspan="3"><strong>ABM Usuarios</strong></td>
        </tr>
        <tr>
            <td class="auto-style13">Usuario:</td>
            <td class="auto-style14">
                <asp:TextBox ID="TxtUsu" runat="server" Width="222px"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="BtnBuscar" runat="server" OnClick="BtnBuscar_Click" Text="Buscar" />
            </td>
        </tr>
        <tr>
            <td class="auto-style15">Pass:</td>
            <td class="auto-style16">
                <asp:TextBox ID="TxtPass" runat="server" TextMode="Password" Width="222px"></asp:TextBox>
            </td>
            <td class="auto-style17">
                <asp:Button ID="BtnLimpiar" runat="server" OnClick="BtnLimpiar_Click" Text="Limpiar" />
            </td>
        </tr>
        <tr>
            <td class="auto-style15">
                <asp:Button ID="BtnAlta" runat="server" Text="Alta" OnClick="BtnAlta_Click" />
            </td>
            <td class="auto-style16">
                <asp:Button ID="BtnModificar" runat="server" Text="Modificar" OnClick="BtnModificar_Click" />
            </td>
            <td class="auto-style17">
                <asp:Button ID="BtnEliminar" runat="server" Text="Eliminar" OnClick="BtnEliminar_Click" />
            </td>
        </tr>
        <tr>
            <td class="auto-style17" colspan="3">
                <asp:Label ID="LblError" runat="server" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Content>

