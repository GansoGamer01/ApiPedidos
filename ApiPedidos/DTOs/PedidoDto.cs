namespace ApiPedidos.DTOs
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public int idCliente { get; set; }
        public string Nome { get; set; }
        public List<PedidoItemDTO> PedidoItems { get; set; }
        public DateTime DataPedido { get; set; }
        public int Status { get; set; }
        public decimal ValorTotal { get; set; }
        public string Observacoes { get; set; }
        
    }

    public class PedidoItemDTO
    {
        public int IdProduto { get; set; }
        public string Titulo { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
