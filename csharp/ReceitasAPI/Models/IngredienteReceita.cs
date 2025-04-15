namespace ReceitasAPI.Models
{
    public class IngredienteReceita
    {
        public int ReceitaId { get; set; }
        public Receita Receita { get; set; } = null!;

        public int IngredienteId { get; set; }
        public Ingrediente Ingrediente { get; set; } = null!;

        public decimal Quantidade { get; set; }
    }
}
