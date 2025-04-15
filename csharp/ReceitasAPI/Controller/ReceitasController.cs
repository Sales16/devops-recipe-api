using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasAPI.Data;
using ReceitasAPI.DTOs;
using ReceitasAPI.Models;

namespace ReceitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReceitasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReceitasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Receita>> PostReceita(ReceitaCreateDTO dto)
        {
            var receita = new Receita
            {
                Nome = dto.Nome,
                ModoPreparo = dto.ModoPreparo,
                Ingredientes = dto.Ingredientes.Select(i => new IngredienteReceita
                {
                    IngredienteId = i.IngredienteId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            _context.Receitas.Add(receita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReceita), new { id = receita.Id }, receita);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceitaReadDTO>>> GetReceitas()
        {
            var receitas = await _context.Receitas
                .Include(r => r.Ingredientes)
                    .ThenInclude(ir => ir.Ingrediente)
                .ToListAsync();

            var result = receitas.Select(r => new ReceitaReadDTO
            {
                Id = r.Id,
                Nome = r.Nome,
                ModoPreparo = r.ModoPreparo,
                Ingredientes = r.Ingredientes.Select(i => new IngredienteReceitaReadDTO
                {
                    IngredienteId = i.IngredienteId,
                    NomeIngrediente = i.Ingrediente.Nome,
                    UnidadeMedida = i.Ingrediente.UnidadeMedida,
                    Quantidade = i.Quantidade
                }).ToList()
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReceitaReadDTO>> GetReceita(int id)
        {
            var r = await _context.Receitas
                .Include(r => r.Ingredientes)
                    .ThenInclude(ir => ir.Ingrediente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null) return NotFound();

            var dto = new ReceitaReadDTO
            {
                Id = r.Id,
                Nome = r.Nome,
                ModoPreparo = r.ModoPreparo,
                Ingredientes = r.Ingredientes.Select(i => new IngredienteReceitaReadDTO
                {
                    IngredienteId = i.IngredienteId,
                    NomeIngrediente = i.Ingrediente.Nome,
                    UnidadeMedida = i.Ingrediente.UnidadeMedida,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceita(int id, ReceitaCreateDTO dto)
        {
            var receita = await _context.Receitas
                .Include(r => r.Ingredientes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();

            receita.Nome = dto.Nome;
            receita.ModoPreparo = dto.ModoPreparo;

            _context.IngredientesReceitas.RemoveRange(receita.Ingredientes);

            receita.Ingredientes = dto.Ingredientes.Select(i => new IngredienteReceita
            {
                IngredienteId = i.IngredienteId,
                Quantidade = i.Quantidade
            }).ToList();

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceita(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Ingredientes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();

            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
