using Microsoft.EntityFrameworkCore;
using ReceitasAPI.Models;

namespace ReceitasAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ingrediente> Ingredientes { get; set; }
        public DbSet<Receita> Receitas { get; set; }
        public DbSet<IngredienteReceita> IngredientesReceitas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<IngredienteReceita>()
            .HasKey(ir => new { ir.ReceitaId, ir.IngredienteId });

        modelBuilder.Entity<IngredienteReceita>()
            .HasOne(ir => ir.Receita)
            .WithMany(r => r.Ingredientes)
            .HasForeignKey(ir => ir.ReceitaId);

        modelBuilder.Entity<IngredienteReceita>()
            .HasOne(ir => ir.Ingrediente)
            .WithMany()
            .HasForeignKey(ir => ir.IngredienteId);
        }
    }
}
