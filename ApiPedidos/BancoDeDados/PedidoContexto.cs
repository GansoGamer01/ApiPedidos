using ApiPedidos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiPedidos.BancoDeDados
{
    public class PedidoContexto : DbContext
    {

        public PedidoContexto(DbContextOptions<PedidoContexto> options) : base(options)
        {

        }

        // criar as variaveis que representam as tabelas \\
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        // para configurar os relacionamentos das tabelas \\
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacionamento entre PedidoItem e Pedido
            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Pedido)
                .WithMany(pe => pe.PedidoItems)
                .HasForeignKey(f => f.PedidoId)  
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento entre PedidoItem e Produto
            modelBuilder.Entity<PedidoItem>()
                .HasOne(pi => pi.Produto)
                .WithMany()
                .HasForeignKey(f => f.ProdutoId)  
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany() 
                .HasForeignKey(p => p.ClienteId)  
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
