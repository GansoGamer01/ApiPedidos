using ApiPedidos.Modelos;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiPedidos.BancoDeDados
{
    public class PedidoContexto : DbContext
    {
        // criar as variaveis que representam as tabelas \\
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        public PedidoContexto(DbContextOptions<PedidoContexto> options) : base(options)
        {

        }

        // para configurar os relacionamentos das tabelas \\
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>()
                .HasMany<PedidoItem>()
                .WithOne(pi => pi.Pedido)
                .HasForeignKey(pi => pi.idPedido);

            
            modelBuilder.Entity<PedidoItem>()
                .HasOne<Pedido>()
                .WithMany(pi => pi.PedidoItem)
                .HasForeignKey(pi => pi.IdPedido)
                .OnDelete(DeleteBehavior.NoAction);

            
            modelBuilder.Entity<Produto>()
                .HasOne(ci => ci.PedidoCozinha)
                .WithMany(ci => ci.PedidoCozinhaItems)
                .HasForeignKey(pci => pci.PedidoCozinhaId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
