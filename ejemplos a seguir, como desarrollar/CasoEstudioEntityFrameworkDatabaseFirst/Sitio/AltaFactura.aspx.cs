using System;
using System.Collections.Generic;
using System.Linq;

using ModeloEF;

public partial class AltaFactura : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                LimpioFormulario();
                DesactivoBotones();
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    private void DesactivoBotones()
    {
        //botn alta lineas: lo activo asi se pueden ingresar lineas para la nueva factura
        btnAgregarArticulo.Enabled = true;

        //no se activa el alta factura hasta que tenga al menos una linea
        btnAgregar.Enabled = false;
        
    }

    private void LimpioFormulario()
    {
        //limpiamos las cajitas y label
        txtNro.Text = "";
        txtFecha.Text = DateTime.Now.Date.ToShortDateString();
        txtCodigoArticulo.Text = "";
        txtCantidad.Text = "";
        lblError.Text = "";

        //generamos coleccion en memoria para ir acumulando las lineas que se dan de alta 
        Session["ListaLineas"] = new List<LINEAS>();

        //limpiamos la grilla - se comienza una factura sin lineas
        gvProductos.DataSource = null;
        gvProductos.DataBind();
    }

    protected void btnAgregarArticulo_Click(object sender, EventArgs e)
    {
        try
        {
            //primero obtengo el DBContext para trabajar
            VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

            //me traigo la coleccin de lineas de la factura que tengo en memoria
            List<LINEAS> _listaL = Session["ListaLineas"] as List<LINEAS>;

            //primero debo buscar el articulo, ya que el usuario solo ingresa su codigo
            // --> es una busqueda individual: DbSet.Where(cond PK).FirstOrDefault()
            // --> IMPORTANTE tener el obejto ORIGINAL ENTITY - esto es para luego poder manejar el alta en la bd 
            int codigo = Convert.ToInt32(txtCodigoArticulo.Text);
            Articulos unArt = Micontexto.Articulos.Where(A => A.CodArt == codigo).FirstOrDefault();

            //si no lo encontre no puedo seguir
            if (unArt == null)
                throw new Exception("El articulo no existe - Intente nuevamente");

            //dado el articulo y la cantidad ya puedo armar una linea
            // --> problema: la tabla tiene como PK la factura y el articulo == no puedo tener dos lineas con el mismo articulo
            // --> solucion  si ya tengo una linea con ese articulo, le sumo la nueva cantidad y listo

            //primero: reviso si ya tengo liena con ese articulo
            //es un List generic, puedo usar la misma formula: Where().FirstOrDefautl();
            LINEAS unaL = _listaL.Where(L => L.Articulos.CodArt == unArt.CodArt).FirstOrDefault();

            if (unaL == null)  //no hay linea con articulo - ES NUEVA LINEA
            {
                //creo objeto linea
                LINEAS _LineaNueva = new LINEAS() //solo tenemos constructor por defecto
                {
                    Articulos = unArt,
                    Cant = Convert.ToInt32(txtCantidad.Text)
                };

                //validar el objeto
                Validar.ValidarLinea(_LineaNueva);

                //agregamos a la coleccion
                _listaL.Add(_LineaNueva);
            }
            else //ya hay una linea con ese articulo - acumulo contidades 
            {
                if (unaL.Cant + Convert.ToInt32(txtCantidad.Text) < 0)
                    throw new Exception("No se puede aplicar el cambio, quedaria cantidad vendida negativa");

                //acumulo cantidad
                unaL.Cant += Convert.ToInt32(txtCantidad.Text);
                Validar.ValidarLinea(unaL);  //validamos pq se cambio la cantidad y podria quedar negativa!  
                //no tengo que tocar la lista, ya que modifique solo uno de sus objetos
            }


            //no importa si es linea nueva o actualizacion...
            //debo actualizar la grilla
            gvProductos.DataSource = _listaL;
            gvProductos.DataBind();
            //ya tengo seguro una linea... habilito el boton para dar de alta la factura
            btnAgregar.Enabled = true;
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnLimpiar_Click(object sender, EventArgs e)
    {
        LimpioFormulario();
        DesactivoBotones();
    }

    protected void btnAgregar_Click(object sender, EventArgs e)
    {
        //1 - debo armar el objeto factura completo - va con try
        //    si hay problemas no puedo seguir y usar el Contexto - me voy
        //2 - trato de dar de alta la factura a traves del EF - va con try
        //    si hay error DEBO QUITAR el objeto del EF para que no siga dando error

        //variables que debo ver en ambos try 
        VentasEntities Micontexto = null;
        Facturas unaF = null;

        //primer paso: creo el objeto factura
        try
        {
            //primero obtengo el DBContext para trabajar
            Micontexto = Application["Micontexto"] as VentasEntities;

            //creo el obejto factura 
            unaF = new Facturas() //solo tengo constructor por defecto
            {
                NumFact = Convert.ToInt32(txtNro.Text),
                FechaFact = Convert.ToDateTime(txtFecha.Text),
                Usuarios = Session["Usuario"] as Usuarios,
                LINEAS = Session["ListaLineas"] as List<LINEAS>
            };

            //validar la factura
            Validar.ValidarFactura(unaF);

            //el numero de la factura no es autogenerado y es la PK de la tabla - no se puede repetir
            Boolean repetido = Micontexto.Facturas.Where(F => F.NumFact == unaF.NumFact).Any();
            if (repetido)
                throw new Exception("Ese numero de factura ya fue ingresado - Intente nuevamente");
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            return; //no tengo objeto Factura completo y correcto - me voy no hago nada mas 
        }


        //segundo paso: intento agregar a la bd
        try
        {
            //agrego la factura al EF
            Micontexto.Facturas.Add(unaF);
            //esto que implica? 
            //  -- el objeto Factura se agrega al DBset - se convierte en Entity --> State = Added --> insert en bd
            //  -- vio que el objeto factura viene con otros objetos!!!
            //     -- detecta que el objeto usuario ya es una Entity - no le pone State - ya existe lo usa solamente
            //     -- detecta la colecicon de objetos lineas.... SON TODO NUEVOS
            //         convierte cada objeto linea en Entity --> State = Added  --> insert en la bd
            //         detecta que cada objeto linea tiene un articulo -- que ya es una Entity - no le pone State - lo usa 
            // --> una factura con 2 lineas ---> implica que el detecta que tiene que hacer 3 INSERT


            //mando a impactar a la bd
            Micontexto.SaveChanges();
            //aca ejecuta todos esos insert SIN TRN
            //si todo sale ok - el propio EF ya cambia el estado de todos esos objetos a Unchanged


            //si llego aca salio todo ok
            LimpioFormulario();
            DesactivoBotones();
            lblError.Text = "Alta con exito";
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
            //debo cambiar el estado.. de quien? de la factura... y se va con sus lineas! 
            Micontexto.Entry(unaF).State = System.Data.Entity.EntityState.Detached;
        }
    }
}