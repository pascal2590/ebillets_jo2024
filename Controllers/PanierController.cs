using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ebillets_jo2024_API.Data;
using ebillets_jo2024_API.Models;

namespace ebillets_jo2024_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanierController(ApplicationDbContext context) : ControllerBase
    {
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Panier>>> GetPaniers()
        {
            return await _context.Paniers.Include(p => p.Utilisateur).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Panier>> GetPanier(int id)
        {
            var panier = await _context.Paniers
                .Include(p => p.Utilisateur)
                .Include(p => p.PaniersOffres) // Remplacé PanierOffres en PaniersOffres le 12/10/2025 voir fichier "PaniersOffres.cs"
                .ThenInclude(po => po.Offre)
                .FirstOrDefaultAsync(p => p.IdPanier == id);

            if (panier == null)
                return NotFound();

            return panier;
        }

        [HttpPost]
        public async Task<ActionResult<Panier>> PostPanier(Panier panier)
        {
            _context.Paniers.Add(panier);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPanier), new { id = panier.IdPanier }, panier);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePanier(int id)
        {
            var panier = await _context.Paniers.FindAsync(id);
            if (panier == null)
                return NotFound();

            _context.Paniers.Remove(panier);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
