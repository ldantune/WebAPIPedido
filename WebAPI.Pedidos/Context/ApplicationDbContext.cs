using Microsoft.EntityFrameworkCore;
using WebAPI.Pedidos.Entities;

namespace WebAPI.Pedidos.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<PedidoProduto> PedidoProdutos { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Produtos)
            .WithOne(pp => pp.Pedido)
            .HasForeignKey(pp => pp.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PedidoProduto>()
            .HasKey(pp => new { pp.PedidoId, pp.ProdutoId });
    }

}
