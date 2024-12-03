using ApiPedidos.BancoDeDados;
using ApiPedidos.DTOs;
using ApiPedidos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoContexto _context;

        public PedidosController(PedidoContexto contexto)
        {
            _context = contexto;
        }

        [HttpGet]
        public async Task<IActionResult> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.PedidoItems)
                .ThenInclude(pi => pi.Produto)
                .ToListAsync();

            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.PedidoItems)
                .ThenInclude(pi => pi.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            var pedidoDTO = MapearPedidoParaDTO(pedido);

            return Ok(pedidoDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, Pedido pedidoAtualizado)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.PedidoItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound("Pedido não encontrado.");
            }

            if (id != pedido.Id)
            {
                return BadRequest("O ID do pedido não corresponde ao ID da URL.");
            }

            // Atualizar campos do pedido
            pedido.ClienteId = pedidoAtualizado.ClienteId;
            pedido.Status = pedidoAtualizado.Status;
            pedido.DataPedido = pedidoAtualizado.DataPedido;
            pedido.Observacoes = pedidoAtualizado.Observacoes;

            // Atualizar itens do pedido
            pedido.PedidoItems.Clear();
            pedido.PedidoItems = pedidoAtualizado.PedidoItems.Select(item => new PedidoItem
            {
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario
            }).ToList();

            // Recalcular o valor total
            pedido.ValorTotal = pedido.PedidoItems.Sum(item => item.Quantidade * item.PrecoUnitario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao atualizar o pedido.");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostPedido(PedidoDTO pedidoDTO)
        {
            if (pedidoDTO == null || pedidoDTO.idCliente <= 0)
            {
                return BadRequest("Dados do pedido inválidos.");
            }

            // Verificar se o cliente existe
            var cliente = await _context.Clientes.FindAsync(pedidoDTO.idCliente);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado.");
            }

            // Verificar se todos os produtos existem
            foreach (var item in pedidoDTO.PedidoItems)
            {
                var produto = await _context.Produtos.FindAsync(item.IdProduto);
                if (produto == null)
                {
                    return NotFound($"Produto com ID {item.IdProduto} não encontrado.");
                }
            }

            // Criar o pedido
            var pedido = new Pedido
            {
                ClienteId = pedidoDTO.idCliente,
                PedidoItems = pedidoDTO.PedidoItems.Select(item => new PedidoItem
                {
                    ProdutoId = item.IdProduto,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario
                }).ToList(),
                Status = 1,
                DataPedido = DateTime.Now,
                Observacoes = pedidoDTO.Observacoes,
                ValorTotal = pedidoDTO.PedidoItems.Sum(item => item.Quantidade * item.PrecoUnitario)
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.Id }, pedido);
        }

        // Método auxiliar para mapear Pedido para PedidoDTO
        private PedidoDTO MapearPedidoParaDTO(Pedido pedido)
        {
            return new PedidoDTO
            {
                Id = pedido.Id,
                idCliente = pedido.ClienteId,
                Nome = pedido.Cliente?.Nome,
                PedidoItems = pedido.PedidoItems.Select(pi => new PedidoItemDTO
                {
                    IdProduto = pi.ProdutoId,
                    Titulo = pi.Produto?.Titulo,
                    Quantidade = pi.Quantidade,
                    PrecoUnitario = pi.PrecoUnitario
                }).ToList(),
                Status = pedido.Status,
                DataPedido = pedido.DataPedido,
                ValorTotal = pedido.ValorTotal,
                Observacoes = pedido.Observacoes
            };
        }
    }
}