using ebillets_jo2024.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "RequireAdmin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public AdminController(ApplicationDbContext context) { _context = context; }

    [HttpGet("VentesParOffre")]
    public async Task<IActionResult> VentesParOffre()
    {
        // Si tu veux mapper la vue dans EF, crée une classe DTO et utilise FromSqlRaw
        var ventes = await _context.Set<VentesParOffreDTO>()
            .FromSqlRaw("SELECT * FROM v_ventesparoffre")
            .ToListAsync();
        return Ok(ventes);
    }
}

public class VentesParOffreDTO
{
    public int idOffre { get; set; }
    public string nomOffre { get; set; }
    public long nbBilletsVendus { get; set; }
    public decimal totalVentes { get; set; }
}
