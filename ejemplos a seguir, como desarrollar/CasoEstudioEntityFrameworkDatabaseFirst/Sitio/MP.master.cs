using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MP : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //validamos que exista objeto dle tipo concreto
            if (!(Session["Usuario"] is ModeloEF.Usuarios))
                Response.Redirect("~/default.aspx");
        }
        catch (Exception)
        {
            Response.Redirect("~/default.aspx");
        }

    }

    protected void BtnDeslogueo_Click(object sender, EventArgs e)
    {
        Session["Usuario"] = null;
        Response.Redirect("~/default.aspx"); //oagina en donde esta el logueo
    }
}
