namespace ApiPedidos.DTOs
{
    public class PedidoGetDto
    {
        public int Id { get; internal set; }

        public string Cliente { get; set; }

        public int ValorTotal { get; set; }

        public List<PedidoItemsGetDto> pedidoItems { get; set; } = new List<PedidoItemsGetDto>();
    }

    public class PedidoItemsGetDto
    {
        public int Id { get; set; }

        public string Item { get; set; }
    }
}
