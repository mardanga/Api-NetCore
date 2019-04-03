using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contactos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Contactos.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly ContactosContext _context;

        public ContactoController(ContactosContext context) {
            _context = context;
        }

        [Authorize]
        public IEnumerable<Contacto> GetAll()
        {

       

            return _context.Contactos.ToList();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Contacto>> GetById(long id)
        {
            var contacto = await _context.Contactos.FindAsync(id);

            if (contacto == null) {
                return NotFound();
            }
            else
            {
                return contacto;
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Contacto>> Create([FromBody] Contacto contacto)
        {
            if (contacto == null)
                return BadRequest();

            var currentUser = HttpContext.User;
            int years = 0;

            if (currentUser.HasClaim(c => c.Type == "FechaCreado"))
            {
                DateTime date = DateTime.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "FechaCreado").Value);
                years = DateTime.Today.Year - date.Year;
            }

            if (years < 2)
            {
                return Forbid();
            }

            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("getById", new { id = contacto.Id }, contacto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Contacto>> Update(long id, [FromBody] Contacto contacto)
        {
            
            if (contacto == null)
                return NotFound();


            _context.Entry(contacto).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        
            await _context.SaveChangesAsync();

            return CreatedAtAction("getById", new { id = contacto.Id }, contacto);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Contacto>> Delete(long id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto == null)
                return NotFound();


            _context.Contactos.Remove(contacto);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}