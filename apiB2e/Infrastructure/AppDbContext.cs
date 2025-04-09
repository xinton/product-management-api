using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;

namespace apiB2e.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Product entity - Code First
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Produto");
                entity.HasKey(e => e.IdProduto);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Valor).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.DataInclusao).IsRequired();
            });

            // Configure User entity - Code First
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Usuario");
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.Login).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Senha).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DataInclusao).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}