using Microsoft.EntityFrameworkCore;


namespace Contactos.Models
{
    public class ContactosContext: DbContext
    {
        public ContactosContext(DbContextOptions<ContactosContext> options)
        : base(options){

        }

        public DbSet<Contacto> Contactos { get; set; }
        public DbSet<User> Users { get; set; }
    }
}