using ebillets_jo2024_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ebillets_jo2024_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Renvoie le nombre de billets vendus et le montant total par offre.
        /// </summary>
        [HttpGet("ventes-par-offre")]
        public async Task<IActionResult> GetVentesParOffre()
        {
            try
            {
                var result = await _context.Offres
                    .Select(o => new
                    {
                        o.IdOffre,
                        NomOffre = o.NomOffre,
                        Prix = o.Prix,
                        NbVentes = _context.Billets
                            .Count(b => b.IdOffre == o.IdOffre),
                        MontantTotal = _context.Billets
                            .Where(b => b.IdOffre == o.IdOffre)
                            .Count() * o.Prix
                    })
                    .OrderByDescending(x => x.MontantTotal)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur serveur : {ex.Message}");
            }
        }
    }
}
