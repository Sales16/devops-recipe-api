using System.ComponentModel.DataAnnotations;

namespace ReceitasAPI.Models
{
    public class Receita
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        public string ModoPreparo { get; set; } = string.Empty;

        public List<IngredienteReceita> Ingredientes { get; set; } = new();
    }
}
