using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ModeloEF;
public partial class ListarFacturas : System.Web.UI.Page
{
    //aunque estoy usando el EF, los listados puedo hacerlos igual que antes: con listas para reuso
    //esto es fundamental, para que el contenido de la grilla no vaya cambiando en el tiempo y de problemas 

    //variables de reuso
    List<Articulos> _ListaArticulos = null;  //para carga drop y ver seleccion
    List<Usuarios> _ListaUsuarios = null;   //para carga del drop y ver seleccion
    List<Facturas> _ListaFacturas = null;    //cargar grilla facturas y fuente de datos para filtro
    List<Facturas> _ListaFiltro = null;      //resultado de los filtros - se carga grilla 

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)  //es el primer ingreso - cargo variables y la UI
            {
                //primero obtengo el DBContext para trabajar
                VentasEntities Micontexto = Application["Micontexto"] as VentasEntities;

                //cargo datos
                Session["ListaArticulos"] = _ListaArticulos = Micontexto.Articulos.ToList(); //fuerza recarga de datos
                Session["ListaUsuarios"] = _ListaUsuarios = Micontexto.Usuarios.ToList();
                Session["ListaFacturas"] = _ListaFacturas = Micontexto.Facturas.ToList();
                //ojo, esto solo recarga objetos facturas, no lineas! - cuando las necesite para una factura la cargo 
                Session["ListaFiltro"] = _ListaFiltro = null; //por defecto no tenemos filtros!

                //cargo elementos de UI (limpieza incluida) 
                ResetFormulario();
            }
            else //recargamos varibles desde la memoria
            {
                _ListaArticulos = Session["ListaArticulos"] as List<Articulos>;
                _ListaUsuarios = Session["ListaUsuarios"] as List<Usuarios>;
                _ListaFacturas = Session["ListaFacturas"] as List<Facturas>;
                _ListaFiltro = Session["ListaFiltro"] as List<Facturas>;
            }
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }

    }

    private void ResetFormulario()
    {
        //cargar drop de articulos - con primer lugar "seleccione" (es pq si no quiero usarlo para filtrar) 
        DDLArticulos.DataSource = _ListaArticulos;
        DDLArticulos.DataTextField = "NomArt";   //solo indico que despleiga NO NECESITO EL VALOR DE RETORNO 
        DDLArticulos.DataBind();
        DDLArticulos.Items.Insert(0, "Seleccione Articulo");
        DDLArticulos.SelectedIndex = 0;  //asi saco seleccion!

        //cargar drop de usuarios - con primer lugar "seleccione" (es pq si no quiero usarlo para filtrar) 
        DDLUsuarios.DataSource = _ListaUsuarios;
        DDLUsuarios.DataTextField = "Usulog";   //solo indico que despleiga NO NECESITO EL VALOR DE RETORNO 
        DDLUsuarios.DataBind();
        DDLUsuarios.Items.Insert(0, "Seleccione Usuario");
        DDLUsuarios.SelectedIndex = 0;  //asi saco seleccion!

        //cargar grilla de facturas - con todas 
        gvFacturas.DataSource = _ListaFacturas;
        gvFacturas.DataBind();

        //limpio filtros - empiezo desde 0 
        Session["ListaFiltro"] = _ListaFiltro = null;

        //limpiar la grilla de lineas (por las dudas si se consulto una factura) 
        gvLineas.DataSource = null;
        gvLineas.DataBind();

        //limpio controles
        TxtFecha.Text = "";
        lblError.Text = "";

        //limpio estadisticas
        GvEstadisticas.DataSource = null;
        GvEstadisticas.DataBind();

    }

    protected void dgvFacturas_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            //1 - determina a cual pagina se quiere cambiar el usuario
            gvFacturas.PageIndex = e.NewPageIndex;
            //pagina que muestra la grilla = la pagina que selecciono el usuario

            //2 - determino la fuente de datos de la grilla (todas las facturas o filtro)
            List<Facturas> _listaFuente = (_ListaFiltro == null) ? _ListaFacturas : _ListaFiltro;
            // (condicion) ? QueHagoPorTRUE : QueHagoPorFALSE

            //3 - recargo la grilla par aque genere los datos de la pagina 
            gvFacturas.DataSource = _listaFuente;
            gvFacturas.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnSinFiltro_Click(object sender, EventArgs e)
    {
        try
        {
            ResetFormulario();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void gvFacturas_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //se selecciono una factura, debo mostrar sus lineas
            //NO HAY QUE VALIDAR QUE EL INDICE SEA CORRECTO, pq para caer en el evento SE TUVO QUE SELECCIONAR primer 

            //1 - debo calcular el indice para la lista 
            // (cantidad de paginas * cantidad de reg por pagina) + indice selecciona de la pagina actual 
            int pos = (gvFacturas.PageIndex * gvFacturas.PageSize) + gvFacturas.SelectedIndex;

            //2 - obtengo la factura directamente de la fuente de datos 
            List<Facturas> _listaFuente = (_ListaFiltro == null) ? _ListaFacturas : _ListaFiltro;
            Facturas unaFsel = _listaFuente[pos];

            //ya tengo la factura... que en realidad viene de un DbSet.. es una ENTITY... puedo usar la carga diferida 
            gvLineas.DataSource = unaFsel.LINEAS.ToList(); //recarga la coleccion de lineas SOLO DE ESTA FACTURA
            gvLineas.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnFiltrar_Click(object sender, EventArgs e)
    {
        try
        {
            //por las dudas...
            lblError.Text = "";

            //me traigo la fuente de datos de la que parto siempre al filtrar
            List<Facturas> _listaF = _ListaFacturas;

            //determino filtro usuario - drop
            if (DDLUsuarios.SelectedIndex > 0) //por 0 es el seleccionar! 
            {
                _listaF = (from unaF in _listaF   //el resultado y la fuente de datos es la misma coleccion - ACUMULO 
                           where unaF.Usuarios.Usulog == _ListaUsuarios[DDLUsuarios.SelectedIndex - 1].Usulog
                           //es -1 pq en la primera pos esta el seleccionar en el drop 
                           select unaF).ToList();
            }

            //determino filtro articulo - drop 
            if (DDLArticulos.SelectedIndex > 0) //por 0 es el seleccionar! 
            {
                _listaF = (from unaF in _listaF     //el resultado y la fuente de datos es la misma coleccion - ACUMULO 
                           from unaL in unaF.LINEAS 
                           //el primer from recorre las facturas de la lista
                           //el segundo from se aplica a CADA OBJETO factura que tengo del primer from 
                           where unaL.Articulos.CodArt == _ListaArticulos[DDLArticulos.SelectedIndex - 1].CodArt
                           //es -1 pq en la primera pos esta el seleccionar en el drop 
                           //puedo filtrar por cualquiera de las variables del from
                           select unaF
                           //aca veo ambas cosas, la variable del from y la del segundo fro. puedo usar cualquiera 
                           //como filtro por linea, solo me quedan las facturas de la cual quedaron lienas - quedan enganchadas
                           ).ToList();
            }

            //determino filtro fecha - cajita con formato (no tengo que validarlo) 
            if (TxtFecha.Text.Length > 0)  //hay algo - seguro es fecha correcta
            {
                DateTime fecha = Convert.ToDateTime(TxtFecha.Text); 

                _listaF = (from unaF in _listaF
                           where unaF.FechaFact.Date == fecha.Date
                           select unaF 
                          ).ToList();
            }

            //verifico si hay resultado
            if (_listaF.Count == 0)//nunca seria nula
            {
                //no hay resultados
                lblError.Text = "La consulta no tiene resultado - Intente de nuevo";
                gvFacturas.DataSource = null;
                gvFacturas.DataBind();
            }
            else //si tenemos resultados 
            {
                gvFacturas.DataSource = _listaF;
                gvFacturas.DataBind();
                Session["ListaFiltro"] = _ListaFiltro = _listaF; //me guardo en la session el resutlado por paginacion y seleccion
            }

            //siempre limpio por las dudas las lineas - ya no hay seleccionado nada en factura 
            gvLineas.DataSource = null;
            gvLineas.DataBind(); 
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnEstad1_Click(object sender, EventArgs e)
    {
        //total de ventas por Mes/año - quiero saber el monto total facturado
        // Mes - Año - Monto
        //me tuve que hacer una op que me calcule el monto de una factura X (linea sabe cant y el articulo)
        try
        {
            //resultado: obj anonimos
            //fuente de datos del linq: dbset Facturas
            List<Object> resultado = (from unaF in _ListaFacturas   //fuente de datos: todas las facturas
                                      group unaF by new {unaF.FechaFact.Year, unaF.FechaFact.Month}  //agripo por dos datos obj anon
                                      into grupito
                                      orderby grupito.Key.Year, grupito.Key.Month
                                      select new   //obajeto anonimo RESUMEN de grupo 
                                      {
                                            Mes = grupito.Key.Month,
                                            Año = grupito.Key.Year,
                                            MontoVendido = grupito.Sum(O => Calculos.MontoFactura(O))
                                            //esta operacion es un metodo linq - que hace? lo mismo que hicimos nosotros a mano
                                            //recorrer el grupo para ir sumando los montos de la factura
                                            //(from unaF in grupito     //recorro el grupo que me esta generando el obj anonimo
                                            // select Calculos.MontoFactura(unaF)).Sum() //  sumo todos los montos de una factura 
                                      }
                                     ).ToList<Object>();
            GvEstadisticas.DataSource = resultado;
            GvEstadisticas.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnEstad2_Click(object sender, EventArgs e)
    {
        //listado de articulos - salen todos, y lo que no tengan ventas van con 0 
        //orenado por cantidad vendida 
        try
        {
            List<Object> resultado = (from unA in _ListaArticulos   //fuente de datos: el dbset de articulos
                                                                    //no hay filtro, no agrupacion, ni orden
                                      select new   //creo un ob anonimo pa los datos que necestio no estan en nignuna clase
                                      {
                                          Codigo = unA.CodArt,
                                          Nombre = unA.NomArt,
                                          Precio = unA.PreArt,
                                          CantidadVendida = //necesito sub consutla pq necesito las facturas
                                          (from unaF in _ListaFacturas   //recorro todas las facturas 
                                           from unaL in unaF.LINEAS      //por cada factura debo revisar sus lineas
                                           where unaL.Articulos.CodArt == unA.CodArt  //filtro por el articulo del primer linq
                                           select unaL.Cant  //solo llegan las lineas de ese articulo, necestio la cant vendida
                                          ).Sum()  //este linq devuelve un conjunto de numeros que necesito ir acumulando
                                      }
                                      ).OrderByDescending(O => O.CantidadVendida).ToList<Object>();

            GvEstadisticas.DataSource = resultado;
            GvEstadisticas.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }

    protected void BtnEstad3_Click(object sender, EventArgs e)
    {
        //listado de vendedores - montos que vendio cada uno 
        //ordenado por monto vendido (descending)
        try
        {
            List<Object> resultado = (from unU in _ListaUsuarios  //dbset con todos los usuarios del sistema
                                      select new
                                      {
                                          Nombre = unU.Usulog,
                                          MontoVendido = (from unaF in _ListaFacturas
                                                          where unaF.Usuarios.Usulog == unU.Usulog
                                                          select Calculos.MontoFactura(unaF)).Sum()
                                      }
                                      ).OrderByDescending(O => O.MontoVendido).ToList<Object>();
            GvEstadisticas.DataSource = resultado;
            GvEstadisticas.DataBind();
        }
        catch (Exception ex)
        {
            lblError.Text = ex.Message;
        }
    }
}