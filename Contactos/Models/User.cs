using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Contactos.Models
{
    public class User
    {
        [Key]
        [Required]
        [Display(Name = "Username")]
        [StringLength(20, ErrorMessage = "El valor para {0] debe contener al menos {2} y máximo {1} caracteres", MinimumLength = 6)]
        public string Username { get; set; }

        

        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaCreado { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }
    }
}
