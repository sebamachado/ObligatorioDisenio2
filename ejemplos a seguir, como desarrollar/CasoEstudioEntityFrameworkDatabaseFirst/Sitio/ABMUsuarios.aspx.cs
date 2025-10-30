using ModeloEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ABMUsuarios : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) //primer ingreso
        {
            LimpiarPantalla();
            DesactivarBotones();
        }
    }

    private void DesactivarBotones()
    {
        BtnBuscar.Enabled = true;
        BtnAlta.Enabled = false;
        BtnModificar.Enabled = false;
        BtnEliminar.Enabled = false; 
    }

    private void LimpiarPantalla()
    {
        TxtUsu.Text = "";
        TxtPass.Text = "";

        TxtPass.Enabled = false;
    }

    protected void BtnLimpiar_Click(object sender, EventArgs e)
    {
        LimpiarPantalla();
        DesactivarBotones();
    }

    protected void BtnBuscar_Click(object sender, EventArgs e)
    {
        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //buscamos si existe el usuario 
            Usuarios unU = Micontexto.Usuarios.Where(U => U.Usulog.Trim() == TxtUsu.Text.Trim()).FirstOrDefault();
            //.Usuarios  --> DBset que contiene los objetos mapeados desde la bd
            //.Where --> realiza busquedas - si no hay datos en el DBset, recarga desde la bd 
            //.FirstOrDefault --> me devuelve objeto si se encontro - nulo de lo contrario

            //en funcion de si encontro o no armo el fomularios
            if (unU == null) //no se encontro - es alta
            {
                TxtPass.Enabled = true;

                BtnAlta.Enabled = true;
                BtnBuscar.Enabled = false;

                Session["ABMUsuario"] = null; //ojo que la la session Usuario es el usuario logueado! 
            }
            else  //se encontro - es B o M
            {
                TxtPass.Enabled = true;

                BtnBuscar.Enabled = false;
                BtnEliminar.Enabled = true;
                BtnModificar.Enabled = true;

                Session["ABMUsuario"] = unU;
            }

        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;
        }
    }

    protected void BtnAlta_Click(object sender, EventArgs e)
    {
        VentasEntities Micontexto = null; //defino por fuera la varible pq necesito verla en ambos try 
        Usuarios unU = null; //defino por fuera la varaible, necesito el usuario en ambos try (uno lo crea el otro lo consume) 

        //primer try - armado del objeto - si hay problemas me voy 
        try
        {
            //primero obtengo el DBContext para trabajar
            Micontexto = Application["Micontexto"] as VentasEntities;

            //debo crear el usuario con los datos que se ingresaron en la UI
            //problema: el EF solo tiene constructor por defecto 
            unU = new Usuarios()    //constructor por defecto - sin parametros
            {//asigna datos al objeto creado vacio - uso directamente las propiedades que se encuentran en el objeto creado
                Usulog = TxtUsu.Text.Trim(),
                PassLog = TxtPass.Text.Trim()
            };

            //validamos al objeto
            Validar.ValidoUsuario(unU);
        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;
            return; //si el objeto tiene errores no puedo ingresarlo al EF - corto la ejecuion y me voy 
        }

        //segundo try - genero alta por medio del EF - con manejo de errores
        try
        {
            Micontexto.Usuarios.Add(unU);
            //primero agrego el objeto al EF - esto hace que el objeto se convierta en una Entity
            //al ser entity tiene ESTADO --> State == Added (realiza un insert) - se define pa es un objeto nuevo 
            //el estado de una entity le indica al EF que accion debe realizar en la bd 
            //los objetos se manipulan a traves del DBset correspondiente
            //OJO esto asi nomas no impacto en la bd aun 

            Micontexto.SaveChanges();
            //le digo al EF que genere los cambios en la bd (impacte en la bd la realidad)
            //lo que hace es revisar TODOS los objetos que tiene en todos los DbSet, y genera las sentencias acordes al estado

            //si llego aca - no hay errores
            LimpiarPantalla();
            DesactivarBotones();
            LblError.Text = "Alta con exito";

        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;

            //hay problemas con la bd al hacer el insert (savechanges + add) 
            Micontexto.Entry(unU).State = System.Data.Entity.EntityState.Detached;
            //si llego aca, me dio error la acicon (en este caso insert) en la bd - la bd quedo bien, no tiene cambios
            //pero el objeto sigue en el contexto y con el state == added
            //y el problema es que el proximo savechanges que se haga ( no importa en donde) INTENTA nuevamente dar alta
            //no es eliminar el objeto (eso provoca state == deleted) 
            //.entry --> busca el objeto dentro del Contexto
            //.State --> reconoce el estado de una entity (objeto que pertenece al EF) 
            // EntityState --> enumerado con todos los tipos de estado q tiene el EF (son 5) 
            //       Detached: indica que es un obejto desachable, se saca del EF sin provocar acciones en la bd, y aca no paso nada 

        }
    }

    protected void BtnModificar_Click(object sender, EventArgs e)
    {
        //solo el propio usuario podra cambiarse la pass 
        //la pass es el unico dato que tengo para cambiar en este concepto 

        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //necesito los dos usuarios: el logueado y el del ABM
            Usuarios unUlogueado = Session["Usuario"] as Usuarios;
            Usuarios unUAbm = Session["ABMUsuario"] as Usuarios;

            //determino si es el propio usuario que quiere cambiar la pass
            if (unUlogueado.Usulog.Trim() != unUAbm.Usulog.Trim())
                throw new Exception("Solo el usuario logueado se podra cambiar la contraseña");

            //si llego aca, es que soy yo mismo quien se cambia la pass 
            //modificamos objeto - uso el que buscamos en el form
            //el que esta logueado no lo toco por si hay problemas, quede con la pass activa 
            unUAbm.PassLog = TxtPass.Text.Trim(); //esta accion cambia el STATE del objeto ENTITY
                                                  //el objeto es Entity pq lo saque del Contexto que maneja a mi EF
                                                  //como modifique el contenido de una propiedad - State = Modified --> ejecutara un update

            //validamos que el objeto este correcto
            Validar.ValidoUsuario(unUAbm);

            //actualizamos la bd
            Micontexto.SaveChanges();

            //si llegue aca es que esta todo ok
            LimpiarPantalla();
            DesactivarBotones();
            LblError.Text = "Se modifico correctamente";

        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;
        }
    }

    protected void BtnEliminar_Click(object sender, EventArgs e)
    {
        //consideraciones...
        //1 - si el usuario que se elimina es el que esta logueado --> debo desloguearlo
        //2 - el usuario tiene relacion con la factura: si el usuario tiene facturas, la bd no deja eliminar. 

        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //obtengo el usuario que se esta manejando en el formulario 
            Usuarios unUAbm = Session["ABMUsuario"] as Usuarios;

            //validamos dependencias - es este caso son las facturas (toda factura sabe que usuario la genero) 
            //esto es un linq to Entity (parece to objetct, pero la fuente de datos es del EF)
            //esto hace que ese dbset se actualize desde la bd para poder usarlo como fuente
            Boolean encontre = (from unaF in Micontexto.Facturas      //fuente de datos: DBSet de facturas
                                where unaF.Usuarios.Usulog.Trim() == unUAbm.Usulog.Trim()  //busco la FK
                                select unaF                           //saco la factura asoc al usuario 
                               ).Any();                               //me devuelve true si el resutlado tiene algo

            //reviso - si hay dependencia no puedo eliminar (la bd no me deja) 
            if (encontre) //variable booleana - ya es una condicion por si misma!
                throw new Exception("No se puede eliminar - Tiene Facturas asoc.");

            //marcamos para eliminar - esto solo cambia el STATE -- > deleted --> delete
            //esta accion NO quita el objeto del DbSet 
            Micontexto.Usuarios.Remove(unUAbm);  //debe ser un objeto que forme parte del EF 

            //impacto en la bd
            Micontexto.SaveChanges();  //si sale todo OK con el delete - el EF saca el objeto del contexto automaticamente

            //reviso si soy yo que me elimine - usu buscado == usu logueado
            if (unUAbm.Usulog.Trim() == ((Usuarios)Session["Usuario"]).Usulog.Trim())
            {
                //me deslogueo
                Session["Usuario"] = null;
                Response.Redirect("~/Default.aspx");
            }

            //si llego aca - elimine un usuario cualquiera
            LimpiarPantalla();
            DesactivarBotones();
            LblError.Text = "Usuario eliminado exitosamente";

        }
        catch (Exception ex)
        {
            LblError.Text = ex.Message;        }
    }
}