using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ebillets_jo2024.Data;
using ebillets_jo2024.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ebillets_jo2024.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PanierController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PanierController(ApplicationDbContext context)
        {
            _context = context;
        }

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
