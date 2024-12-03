namespace ApiPedidos.DTOs
{
    public class PedidoItemDto
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public string TituloProduto { get; set; }  // Ou o nome do produto
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
    }
}
