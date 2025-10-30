using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ModeloEF;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //reviso si ya esta creado el contexto....
            if (Application["Micontexto"] == null)
                Application["Micontexto"] = new VentasEntities();
            //este objeto es la clase DBcontext de mi EF - maneja todo: objetos y consutlas y ABM
            //como lo guarde en la APP, todos los usuarios del sitio lo van a compartir 
        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;
        }
    }



    protected void BtnLogueo_Click(object sender, EventArgs e)
    {
        try
        {
            //primero obtengo el Dbcontext para trabajar con mi EF
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //segundo obtengo los datos para buscar usuario
            string _nombre = TxtUsuario.Text.Trim();
            string _pass = TxtPass.Text.Trim();

            //busco a ver si estan bien las credenciales de ingreso
            //en lugar de logica usamos el Dbcontext directamente
            //para busquedas o listados: WHERE  (si no hya datos el se encarga de provovar la carga de datos desde bd)
            //usamos el DBset (propiedad de tipo coleccion) del tipo de datos correspondiente
            //el Where siempre devuelve conjunto - si estoy en un BUSCAR INDIVIDUAL - FirstOrDefault me devuelve
            //                 el primer objeto de la coleccion (encontre lo que buscaba) o null si no se encontro nada 
            //                 esto es igual que los buscar de la logica: objeto o nulo
            Usuarios unU = Micontexto.Usuarios.Where(U => U.Usulog.Trim() == _nombre && U.PassLog.Trim() == _pass).FirstOrDefault();

            //validamos si nos logueamos
            if (unU == null) //credenciales erroneas - no se loguea
            {
                LblError.Text = "Usuario o Contraseña invalidos";
                TxtUsuario.Text = ""; 
            }
            else //si se pudo loguear
            {
                Session["Usuario"] = unU;  //lo de siempre
                Response.Redirect("~/AbmArticulos.aspx");
            }
        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;
        }
    }
}