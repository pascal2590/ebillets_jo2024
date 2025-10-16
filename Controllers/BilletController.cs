using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ebillets_jo2024_API.Data;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BilletController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BilletController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Billet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Billet>>> GetBillets()
        {
            return await _context.Billets
                .Include(b => b.Offre)
                .Include(b => b.Reservation)
                .ToListAsync();
        }

        // GET: api/Billet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Billet>> GetBillet(int id)
        {
            var billet = await _context.Billets
                .Include(b => b.Offre)
                .Include(b => b.Reservation)
                .FirstOrDefaultAsync(b => b.IdBillet == id);

            if (billet == null)
                return NotFound();

            return billet;
        }

        // POST: api/Billet
        [HttpPost]
        public async Task<ActionResult<Billet>> PostBillet(Billet billet)
        {
            _context.Billets.Add(billet);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBillet), new { id = billet.IdBillet }, billet);
        }

        // PUT: api/Billet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillet(int id, Billet billet)
        {
            if (id != billet.IdBillet)
                return BadRequest();

            _context.Entry(billet).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Billet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBillet(int id)
        {
            var billet = await _context.Billets.FindAsync(id);
            if (billet == null)
                return NotFound();

            _context.Billets.Remove(billet);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
