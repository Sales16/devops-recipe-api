namespace ReceitasAPI.DTOs
{
    public class ReceitaCreateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string ModoPreparo { get; set; } = string.Empty;
        public List<IngredienteQuantidadeDTO> Ingredientes { get; set; } = new();
    }
}
