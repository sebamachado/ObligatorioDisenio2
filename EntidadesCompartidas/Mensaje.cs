using System;
using System.Collections.Generic;
using System.Linq;

namespace EntidadesCompartidas
{
    public abstract class Mensaje
    {
        //Atributos
        private int _id;
        private string _asunto;
        private string _texto;
        private DateTime _fechaEnvio;
        private string _remitente;
        private List<Usuario> _destinatarios;

        //Propiedades
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Asunto
        {
            get { return _asunto; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Error en Asunto del mensaje");
                _asunto = value;
            }
        }

        public string Texto
        {
            get { return _texto; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Error en Texto del mensaje");
                _texto = value;
            }
        }

        public DateTime FechaEnvio
        {
            get { return _fechaEnvio; }
            set { _fechaEnvio = value; }
        }

        public string Remitente
        {
            get { return _remitente; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Error en Remitente del mensaje");
                _remitente = value;
            }
        }

        public List<Usuario> Destinatarios
        {
            get { return _destinatarios; }
            set
            {
                if (value == null)
                    throw new Exception("Error en Destinatario");
                if (value.Count == 0)
                    throw new Exception("Debe existir Destinatarios");
                _destinatarios = value;
            }
        }

        //Constructor completo
        protected Mensaje(string asunto,string texto,string remitente,List<Usuario> destinatarios)
        {
            Asunto = asunto;
            Texto = texto;
            Remitente = remitente;
            FechaEnvio = DateTime.Now;
            Destinatarios = destinatarios;
        }
        public override string ToString()
        {
            return Asunto + " - " + Texto + " - " + Remitente + " - " + FechaEnvio;
        }
    }
}
