using System;
using System.Collections.Generic;
using System.Linq;


using ModeloEF;
using System.Data.SqlClient;

public partial class AbmArticulos: System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LimpiarPantalla();
            DesactivarBotones();
        }
    }

    private void LimpiarPantalla()
    {
        txtCodigo.Text = "";
        txtNombre.Text = "";
        txtPrecio.Text = "";
        lblError.Text = "";

        txtNombre.Enabled = false;
        txtPrecio.Enabled = false;
    }

    private void DesactivarBotones()
    {
        BtnBuscar.Enabled = true;
        BtnAlta.Enabled = false;
        BtnModificar.Enabled = false;
        BtnBaja.Enabled = false;
    }

    protected void BtnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarPantalla();
        DesactivarBotones();
    }

    protected void BtnListar_Click(object sender, EventArgs e)
    {
        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //le debo pedir al EF que me de todos los articulos
            //Dbset.ToList() 
            gvListado.DataSource = Micontexto.Articulos.ToList(); //esto provoca la recarga del DBset si es necesario 
            gvListado.DataBind();

        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnBuscar_Click(object sender, EventArgs e)
    {
        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //buscar el articulo por codigo
            //EF - busqueda indivdual - Where(cond).FirstOrDefault()
            int codigo = Convert.ToInt32(txtCodigo.Text);
            Articulos unArt = Micontexto.Articulos.Where(A => A.CodArt == codigo).FirstOrDefault();

            //verifico que accion realizar
            BtnBuscar.Enabled = false;
            if (unArt == null) //no existe - alta
            {
                txtNombre.Enabled = true;
                txtPrecio.Enabled = true;
                BtnAlta.Enabled = true;
                Session["ABMArticulo"] = null;
            }
            else //exixte es B o M
            {
                txtNombre.Enabled = true;
                txtNombre.Text = unArt.NomArt;
                txtPrecio.Enabled = true;
                txtPrecio.Text = unArt.PreArt.ToString();

                BtnModificar.Enabled = true;
                BtnBaja.Enabled = true;

                Session["ABMArticulo"] = unArt;
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnAlta_Click(object sender, EventArgs e)
    {
        //para el alta en la bd tenemos un SP 
        //el alta sera manual (nosotros armamos todo) y si sale todo bien, actualizamos el EF

        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //debo crear el objeto y validarlo 
            Articulos unArt = new Articulos()
            {
                CodArt = Convert.ToInt32(txtCodigo.Text),
                NomArt = txtNombre.Text.Trim(),
                PreArt = Convert.ToInt32(txtPrecio.Text)
            };
            Validar.ValidarArticulo(unArt);

            //uso el SP para dar de alta al articulo
            //para ejecutar el SP necesito pasarle los datos por parametro: SqlParameter
            SqlParameter _codigo = new SqlParameter("@cod", unArt.CodArt);
            SqlParameter _nombre = new SqlParameter("@nom",unArt.NomArt);
            SqlParameter _precio = new SqlParameter("@pre",unArt.PreArt);

            //el EF no reconoce el returnValue, pero pusimos un parametro output para obtener los errores
            SqlParameter _retorno = new SqlParameter("@ret", System.Data.SqlDbType.Int);
            _retorno.Direction = System.Data.ParameterDirection.Output; //por defecto se crea de tipo input (solo ida de datos) 


            //armamos y ejecutamos el SP - todo a traves del EF que es quien sabe cual bd, donde esta y como conectar
            //.database --> sabe bd, donde esta y como conectar
            //.ExecuteSqlCommand --> crea un objeto Sqlcommand y lo ejecuta
            //      primer param: que debe ejecutar el comando 
            //      segundo param en adelante: los sqlparameter con datos! deben ir en el mismo orden que en la sentencia
            Micontexto.Database.ExecuteSqlCommand("exec AltaArticulo @cod, @nom, @pre, @ret output", 
                                                                             _codigo, _nombre, _precio, _retorno);

            //revisamos si hay errores
            if ((int)_retorno.Value == -1) //usar un parametro output es identico a uno de returnvalue
                throw new Exception("Codigo Existente - No se da de alta ");

            //si llego aca salio todo ok 

            //primero actualizo el EF para que ya cargue el nuevo articulo
            Micontexto.Articulos.ToList();

            //doy mensaje
            LimpiarPantalla();
            DesactivarBotones();
            lblError.Text = "Alta con exito";

        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            //no tengo que hacer nada si hay error en el SP - pq jamas toque al contexto! 
        }
    }

    protected void BtnBaja_Click(object sender, EventArgs e)
    {
        //para la baja en la bd tenemos un SP 
        //la baja sera manual (nosotros armamos todo) y si sale todo bien, actualizamos el EF

        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //debo obtener el objeto seleccionado para eliminar - Necesito el original del EF
            Articulos unArt = Session["ABMArticulo"] as Articulos;

            //no necesito validar dependencias - ya esta incluido en el SP !!!!!!!!!!!

            //uso el SP para dar de baja al articulo
            //para ejecutar el SP necesito pasarle los datos por parametro: SqlParameter
            SqlParameter _codigo = new SqlParameter("@cod", unArt.CodArt);

            //el EF no reconoce el returnValue, pero pusimos un parametro output para obtener los errores
            SqlParameter _retorno = new SqlParameter("@ret", System.Data.SqlDbType.Int);
            _retorno.Direction = System.Data.ParameterDirection.Output; //por defecto se crea de tipo input (solo ida de datos) 


            //armamos y ejecutamos el SP - todo a traves del EF que es quien sabe cual bd, donde esta y como conectar
            //.database --> sabe bd, donde esta y como conectar
            //.ExecuteSqlCommand --> crea un objeto Sqlcommand y lo ejecuta
            //      primer param: que debe ejecutar el comando 
            //      segundo param en adelante: los sqlparameter con datos! deben ir en el mismo orden que en la sentencia
            Micontexto.Database.ExecuteSqlCommand("exec BajaArticulo @cod, @ret output", _codigo, _retorno);

            //revisamos si hay errores
            if ((int)_retorno.Value == -1) //usar un parametro output es identico a uno de returnvalue
                throw new Exception("No existe el articulo - No se elimina ");
            else if ((int)_retorno.Value == -2)
                throw new Exception("Articulo Facturado - No se puede eliminar ");

            //si llego aca salio todo ok 

            //si llego aca se elimino el registro en la bd
            //tengo que sacar el objeto del EF - no puedo usar remove pq eso genera sentencia para ejecutar
            Micontexto.Entry(unArt).State = System.Data.Entity.EntityState.Detached; //con esto desaparece sin accion en sql
            Micontexto.SaveChanges(); //en realidad no hace nada en la bd... pero saca al objeto que sobra

            //doy mensaje
            LimpiarPantalla();
            DesactivarBotones();
            lblError.Text = "Baja con exito";
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnModificar_Click(object sender, EventArgs e)
    {
        //necesito las variables en el try y en el catch
        VentasEntities Micontexto = null;
        Articulos unArt = null;

        try
        {
            //primero obtengo el DBContext para trabajar
            Micontexto = Application["Micontexto"] as VentasEntities;

            //segundo me traigo el objeto Entity que busque y guarde (necesito el original!)
            unArt = Session["ABMArticulo"] as Articulos;

            //para modificar NO hay operacion - cambio el contenido del articulo directamente 
            //esto provoca que se cambie el State a Modified ---> update
            unArt.NomArt = txtNombre.Text.Trim(); //con esto ya queda como modificado
            unArt.PreArt = Convert.ToInt32(txtPrecio.Text);
            //IMPORTANTE!!!!!!!!!!!  --> jamas tocar el atributo que es la PK en la bd!!!!!!

            //validamos al objeto
            Validar.ValidarArticulo(unArt);

            //si llego aca es valido - mando a impactar en la bd 
            Micontexto.SaveChanges(); //si esto no da error el propio EF le cambia el estado a UnChanged al objetito 

            //si llego aca salio todo ok
            LimpiarPantalla();
            DesactivarBotones();
            lblError.Text = "Actualizacion Correcta";

        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;

            //aca puede darse el problema de: error en validar o el impacto en la bd
            //esto sucede luego de que se cambio el STATE a modified y queda con ese estado 
            //hay que sacar el objeto del en medio pq no representa al registro 
            Micontexto.Entry(unArt).State = System.Data.Entity.EntityState.Detached;
        }
    }
}
