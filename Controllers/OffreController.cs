using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OffreController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Offre
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Offre>>> GetOffres()
        {
            return await _context.Offres.ToListAsync();
        }

        // GET: api/Offre/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Offre>> GetOffre(int id)
        {
            var offre = await _context.Offres.FindAsync(id);

            if (offre == null)
                return NotFound();

            return offre;
        }

        // POST: api/Offre
        [HttpPost]
        public async Task<ActionResult<Offre>> PostOffre(Offre offre)
        {
            _context.Offres.Add(offre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOffre), new { id = offre.IdOffre }, offre);
        }

        // PUT: api/Offre/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffre(int id, Offre offre)
        {
            if (id != offre.IdOffre)
                return BadRequest();

            _context.Entry(offre).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Offres.Any(e => e.IdOffre == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Offre/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffre(int id)
        {
            var offre = await _context.Offres.FindAsync(id);
            if (offre == null)
                return NotFound();

            _context.Offres.Remove(offre);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
