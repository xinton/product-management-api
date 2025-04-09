namespace apiB2e.Entities
{
    public class Product
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    }
}