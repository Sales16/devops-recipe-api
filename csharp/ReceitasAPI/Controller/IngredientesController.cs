using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceitasAPI.Data;
using ReceitasAPI.Models;

namespace ReceitasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IngredientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetIngredientes()
        {
            var ingredientes = await _context.Ingredientes.ToListAsync();
            return Ok(new { message = $"Foram encontrados {ingredientes.Count} ingredientes.", data = ingredientes });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetIngrediente(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
                return NotFound(new { message = $"Ingrediente com id {id} não encontrado." });

            return Ok(new { message = $"Ingrediente com id {id} encontrado.", data = ingrediente });
        }

        [HttpPost]
        public async Task<IActionResult> PostIngrediente(Ingrediente ingrediente)
        {
            _context.Ingredientes.Add(ingrediente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIngrediente), new { id = ingrediente.Id }, new { message = $"Ingrediente criado com sucesso com id {ingrediente.Id}.", data = ingrediente });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngrediente(int id, Ingrediente ingrediente)
        {
            if (id != ingrediente.Id)
                return BadRequest(new { message = "ID do ingrediente não corresponde ao informado." });

            _context.Entry(ingrediente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Ingredientes.Any(e => e.Id == id))
                    return NotFound(new { message = $"Ingrediente com id {id} não encontrado para atualização." });
                else
                    throw;
            }

            return Ok(new { message = $"Ingrediente com id {id} atualizado com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngrediente(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
                return NotFound(new { message = $"Ingrediente com id {id} não encontrado para exclusão." });

            _context.Ingredientes.Remove(ingrediente);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Ingrediente com id {id} deletado com sucesso." });
        }
    }
}
