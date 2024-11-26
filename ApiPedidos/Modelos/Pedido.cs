using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiPedidos.Modelos
{
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int idCliente { get; set; }
        public virtual ICollection<Cliente> Cliente { get; set; }
        public int DataPedido { get; set; } // mudar para timestamp dps \\
        public int Status { get; set; } = 1;// Status(Pendente; processando; enviado; Entregue; Cancelado) \\  // começa pendente sempre \\
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; }
    }
}
