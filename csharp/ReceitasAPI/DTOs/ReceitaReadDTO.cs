namespace ReceitasAPI.DTOs
{
    public class ReceitaReadDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string ModoPreparo { get; set; } = string.Empty;
        public List<IngredienteReceitaReadDTO> Ingredientes { get; set; } = new();
    }
}
