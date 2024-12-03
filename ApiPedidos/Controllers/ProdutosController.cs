using ApiPedidos.BancoDeDados;
using ApiPedidos.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiPedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly PedidoContexto _context;

        public ProdutosController(PedidoContexto context)
        {
            _context = context;
        }

        // GET: api/produtos
        [HttpGet]
        public async Task<IActionResult> GetProdutos()
        {
            var produtos = await _context.Produtos.ToListAsync();
            return Ok(produtos);
        }

        // GET: api/produtos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProdutoById(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        // POST: api/produtos
        [HttpPost]
        public async Task<IActionResult> PostProduto(Produto produto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProdutoById), new { id = produto.Id }, produto);
        }

        // PUT: api/produtos/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produtoAtualizado)
        {
            if (id != produtoAtualizado.Id)
            {
                return BadRequest("O ID do produto na URL não corresponde ao do corpo da requisição.");
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            produto.Titulo = produtoAtualizado.Titulo;
            produto.Descricao = produtoAtualizado.Descricao;
            produto.Preco = produtoAtualizado.Preco;
            produto.Estoque = produtoAtualizado.Estoque;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Método auxiliar para verificar a existência do produto
        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(p => p.Id == id);
        }
    }
}