using System;
using System.Text.RegularExpressions;

namespace EntidadesCompartidas
{
    public class Usuario
    {
        //Atributos
        private string _username;
        private string _password;
        private string _nombreCompleto;
        private DateTime _fechaNacimiento;
        private string _email;

        //Propiedades
        public string Username
        {
            get { return _username; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Trim().Length != 8)
                    throw new Exception("El nombre de usuario debe tener 8 caracteres");
                _username = value;
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length != 8)
                    throw new Exception("La contraseña debe tener exactamente 8 caracteres");

                int letras = 0, dig = 0, simb = 0;
                foreach (char ch in value)
                {
                    if (char.IsLetter(ch)) letras++;
                    else if (char.IsDigit(ch)) dig++;
                    else simb++;   // cualquier otro → símbolo
                }

                if (letras != 3 || dig != 3 || simb != 2)
                    throw new Exception("La contraseña debe contener 3 letras, 3 dígitos y 2 símbolos");

                _password = value;
            }
        }

        public string NombreCompleto
        {
            get { return _nombreCompleto; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("El nombre completo no puede quedar vacío");
                _nombreCompleto = value;
            }
        }

        public DateTime FechaNacimiento
        {
            get { return _fechaNacimiento; }
            set { _fechaNacimiento = value.Date; }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                const string rx = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(value ?? "", rx))
                    throw new Exception("Email con formato inválido");
                _email = value;
            }
        }

        // ─── Constructor completo ───
        public Usuario(string username,string password,string nombreCompleto,DateTime fechaNacimiento,string email)
        {
            Username = username;
            Password = password;
            NombreCompleto = nombreCompleto;
            FechaNacimiento = fechaNacimiento;
            Email = email;
        }
        public override string ToString()
        {
            return Username + " - " + NombreCompleto + " - " + FechaNacimiento + " - " + Email;
        }
    }
}
