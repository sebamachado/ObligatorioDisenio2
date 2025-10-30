using System;
using System.Collections.Generic;

namespace EntidadesCompartidas
{
    public class Categoria
    {
        //Atributos
        private string _codigo;
        private string _nombre;

        //Propiedades
        public string Codigo
        {
            get { return _codigo; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 3)
                    throw new Exception("Error en Código de categoría");
                _codigo = value;
            }
        }

        public string Nombre
        {
            get { return _nombre; }
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Error en Nombre de categoría");
                _nombre = value;
            }
        }

        //Constructor completo
        public Categoria(string codigo, string nombre)
        {
            Codigo = codigo;
            Nombre = nombre;
        }

        public override string ToString()
        {
            return Codigo + " - " + Nombre;
        }
    }
}
