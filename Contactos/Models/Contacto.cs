using System;

namespace Contactos.Models
{
    public class Contacto
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Email{ get; set; }
        public DateTime? Nace { get; set; }
        public string Mensaje { get; set; }
    }
}