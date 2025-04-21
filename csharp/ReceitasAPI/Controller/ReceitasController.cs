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
        public async Task<IActionResult> PostReceita(ReceitaCreateDTO dto)
        {
            var idsIngredientes = dto.Ingredientes.Select(i => i.IngredienteId).Distinct();
            var ingredientesExistentes = await _context.Ingredientes
                .Where(i => idsIngredientes.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            var ingredientesInvalidos = idsIngredientes.Except(ingredientesExistentes).ToList();

            if (ingredientesInvalidos.Any())
            {
                return BadRequest(new { message = $"Os seguintes IngredienteId não existem: {string.Join(", ", ingredientesInvalidos)}" });
            }

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

            var receitaReadDto = new ReceitaReadDTO
            {
                Id = receita.Id,
                Nome = receita.Nome,
                ModoPreparo = receita.ModoPreparo,
                Ingredientes = receita.Ingredientes.Select(i => new IngredienteReceitaReadDTO
                {
                    IngredienteId = i.IngredienteId,
                    NomeIngrediente = _context.Ingredientes.Find(i.IngredienteId)?.Nome ?? "",
                    UnidadeMedida = _context.Ingredientes.Find(i.IngredienteId)?.UnidadeMedida ?? "",
                    Quantidade = i.Quantidade
                }).ToList()
            };

            return CreatedAtAction(nameof(GetReceita), new { id = receita.Id }, new { message = $"Receita criada com sucesso com id {receita.Id}.", data = receitaReadDto });
        }

        [HttpGet]
        public async Task<IActionResult> GetReceitas()
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
            }).ToList();

            return Ok(new { message = $"Foram encontradas {result.Count} receitas.", data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceita(int id)
        {
            var r = await _context.Receitas
                .Include(r => r.Ingredientes)
                    .ThenInclude(ir => ir.Ingrediente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null)
                return NotFound(new { message = $"Receita com id {id} não encontrada." });

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

            return Ok(new { message = $"Receita com id {id} encontrada.", data = dto });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceita(int id, ReceitaCreateDTO dto)
        {
            var receita = await _context.Receitas
                .Include(r => r.Ingredientes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound(new { message = $"Receita com id {id} não encontrada." });

            var idsIngredientes = dto.Ingredientes.Select(i => i.IngredienteId).Distinct();
            var ingredientesExistentes = await _context.Ingredientes
                .Where(i => idsIngredientes.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            var ingredientesInvalidos = idsIngredientes.Except(ingredientesExistentes).ToList();

            if (ingredientesInvalidos.Any())
            {
                return BadRequest(new { message = $"Os seguintes IngredienteId não existem: {string.Join(", ", ingredientesInvalidos)}" });
            }

            receita.Nome = dto.Nome;
            receita.ModoPreparo = dto.ModoPreparo;

            _context.IngredientesReceitas.RemoveRange(receita.Ingredientes);

            receita.Ingredientes = dto.Ingredientes.Select(i => new IngredienteReceita
            {
                IngredienteId = i.IngredienteId,
                Quantidade = i.Quantidade
            }).ToList();

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Receita com id {id} atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReceita(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Ingredientes)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound(new { message = $"Receita com id {id} não encontrada para exclusão." });

            _context.Receitas.Remove(receita);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Receita com id {id} deletada com sucesso." });
        }
    }
}
