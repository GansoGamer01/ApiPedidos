using ApiPedidos.BancoDeDados;
using ApiPedidos.Modelos;
using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            return await _context.Produtos.ToListAsync();
        }

        // GET: api/produtos/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProdutos(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<ActionResult<Produto>> PostCardapioItem(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProdutos", new { id = produto.Id }, produto);
        }

        // PUT: api/CardapioItems/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProdutos(int id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardapioItemExists(id))
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

        private bool CardapioItemExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
}
