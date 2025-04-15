namespace ReceitasAPI.DTOs
{
    public class IngredienteReceitaReadDTO
    {
        public int IngredienteId { get; set; }
        public string NomeIngrediente { get; set; } = string.Empty;
        public string UnidadeMedida { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
    }
}
