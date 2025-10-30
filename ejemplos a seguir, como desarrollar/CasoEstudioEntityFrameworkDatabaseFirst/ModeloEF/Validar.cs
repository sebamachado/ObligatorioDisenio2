using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModeloEF
{
    //esta clase esta por fuera del modelo EF - significa que si actualizo el modelo no la toca
    //pero esta dentro de la misma biblioteca 

    public class Validar//esto es para que se vea por fuera del componente
    {
        //genero operaciones de clase para validar cada concepto
        //puedo hacerlas de clase, de instancia e incluso armar un singleton
        //son operaciones VOID - lanzan excepction si hay error (ya viene objeto cargado con los datos)
        public static void ValidoUsuario(Usuarios unU)
        {
            //nombre obligatorio - largo max 15c minimo 5c
            if (unU.Usulog.Trim().Length < 5 || unU.Usulog.Trim().Length > 15)
                throw new Exception("Usuario: nombre entre 5 y 15 caracteres");

            //pass obligatoria - largo max de 5c minimo 2c
            if (unU.PassLog.Trim().Length < 2 || unU.PassLog.Trim().Length > 5)
                throw new Exception("Usuario: password entre 2 y 5 caracteres");
        }

        public static void ValidarArticulo(Articulos unA)
        {
            //codigo numerico - obligatorio y mayor a 0
            if (unA.CodArt <= 0)
                throw new Exception("Articulo: codigo numero mayor a 0");

            //precio numerico - obligatorio y mayor 0 
            if (unA.PreArt <= 0)
                throw new Exception("Articulo: precio numero mayor a 0");

            //nombre obligatorio max 25c min 5c
            if (unA.NomArt.Trim().Length < 5 || unA.NomArt.Trim().Length > 25)
                throw new Exception("Articulo: nombre entre 5 y 25 caracteres");

        }

        public static void ValidarFactura(Facturas unaF)
        {
            //el numero es obligatorio y no es autogenerado (> 0)
            //codigo numerico - obligatorio y mayor a 0
            if (unaF.NumFact <= 0)
                throw new Exception("Factura: numero de factura mayor a 0");

            //el usuario que genero la factura  - obligatorio (es prop. de navegacion - pero se valida igual)
            //que sea prop de navegacion significa q no contiene directo el objeto 
            // contiene una referencia a un objeto que existe en un dbset en el Contexto
            //esto es asi pq el EF guarda un SOLO OBJETO que referencia a un registro en la bd
            if (unaF.Usuarios == null)
                throw new Exception("Factura: es obligatorio saber quien genero la factura (usuario)");

            //lineas: debo tener un conjunto y al menos una linea (tengo que vender algo)
            if (unaF.LINEAS == null) //reviso tener un conjunto
                throw new Exception("Factura: es obligatorio saber que se vendio (lineas)");
            if (unaF.LINEAS.Count == 0 ) //reviso tener al menos una linea
                throw new Exception("Factura: es obligatorio saber que se vendio (lineas)");
        }

        public static void ValidarLinea(LINEAS unaL)
        {
            //el articulo qeu se vende es obligatorio
            if (unaL.Articulos == null) //reviso tener objeto
                throw new Exception("Linea: es obligatorio saber que se vendio (articulo)");

            //la cantidad vendida debe ser positiva (>0) 
            if (unaL.Cant <= 0)
                throw new Exception("Linea: cantidad mayor a 0");

        }
    }


    public class Calculos
    {
        public static int MontoFactura(Facturas F)
        {
            int monto = (from unaL in F.LINEAS     //recorro la coleccion de lineas de la Factura que vien por param
                         select (unaL.Cant * unaL.Articulos.PreArt) //necesito calcular el monto que determina cada linea
                         //el select puede devovler un valor (en este caso el resutlado de *), o un objeto
                         ).Sum(); //me suma una coleccion de nuemros. El tipo va en concordancia al tipo que tengamos en la coleccion

            return monto;
        }
    }


}
