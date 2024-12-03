using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ApiPedidos.Modelos
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int PedidoItemId { get; set; }
        public List<PedidoItem> PedidoItems { get; set; } = new List<PedidoItem>();
        public DateTime DataPedido { get; set; } 
        public int Status { get; set; } = 1; // Status(Pendente = 1; processando = 2; enviado = 3; Entregue = 4; Cancelado = 5) \\  // começa pendente sempre \\
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; }
    }
}
