namespace apiB2e.Entities
{
    public class User
    {
        public int IdUsuario { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    }
}