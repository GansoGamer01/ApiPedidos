using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ApiPedidos.Modelos
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int idCliente { get; set; }
        public virtual Cliente Cliente { get; set; }
        public int idPedidoItem { get; set; }
        public virtual ICollection<PedidoItem> PedidoItems { get; set; }
        public DateTime DataPedido { get; set; }
        public int Status { get; set; } = 1; // Status(Pendente = 1; processando = 2; enviado = 3; Entregue = 4; Cancelado = 5) \\  // começa pendente sempre \\
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; }
    }
}
