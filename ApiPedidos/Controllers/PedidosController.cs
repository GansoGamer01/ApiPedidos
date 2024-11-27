using ApiPedidos.BancoDeDados;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ApiPedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        // variavel do banco de dados
        private readonly PedidoContexto _context;
        // o contrutor do controlador 
        public PedidosController(PedidoContexto contexto)
        {
            _context = contexto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoGetDto>>> GetPedidos()
        {
            // SELECT c.NumeroMesa, c.NomeCliente FROM Comandas WHERE SituacaoComanda = 1 \\
            var comandas =
                await _context.Pedidos
                .Where(p => p.Status == 1)
                .Select(c => new PedidoGetDto
                {
                    Id = p.Id,
                    NumeroMesa = p.NumeroMesa,
                    NomeCliente = p.NomeCliente,
                    ComandaItens = p.ComandaItems
                    .Select(ci => new ComandaItensGetDto
                    {
                        Id = ci.Id,
                        Titulo = ci.CardapioItem.Titulo,
                    }
                    ).ToList(),

                }
                ).ToListAsync();

            // retorna o conteudo com uma lista de comandas \\
            return Ok(comandas);
        }

        // GET: api/Comandas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ComandaGetDto>> GetComanda(int id)
        {
            // SELECT * FROM Comandas WHERE id = 1 \\
            // SELECT * FROM ComandaItems WHERE ComandaId = 1 \\
            var comanda = await _context.Comandas.FirstOrDefaultAsync(c => c.Id == id);

            if (comanda == null)
            {
                return NotFound();
            }

            var comandaDto = new ComandaGetDto()
            {
                NumeroMesa = comanda.NumeroMesa,
                NomeCliente = comanda.NomeCliente,
            };

            // SELECT id FROM ComandaItems WHERE ci.ComandaId = 1 \\
            // INNER JOIN CardapioItems cli.id = ci.CardapioItemId \\
            // busca os itens da comanda \\
            var comandaItens =
                    await _context.ComandaItems
                        .Include(ci => ci.CardapioItem)
                            .Where(ci => ci.ComandaId == id)
                                .Select(ci => new ComandaItensGetDto
                                {
                                    Id = ci.Id,
                                    Titulo = ci.CardapioItem.Titulo
                                })
                            .ToListAsync();

            comandaDto.ComandaItens = comandaItens;
            return comandaDto;
        }

        // PUT: api/Comandas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComanda(int id, ComandaUpdateDto comanda)
        {
            if (id != comanda.Id)
            {
                return BadRequest();
            }

            // SELECT * FROM Comandas WHERE id = 2 \\
            var ComandaUpdate = await _context.Comandas.FirstAsync(c => c.Id == id);

            // verifica se foi informada uma nova mesa \\
            if (comanda.NumeroMesa > 0)
            {
                // verificar a disponibilidade da nova mesa \\
                // SELECT * FROM mesas WHERE NumerMesa = 2 \\ 
                var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.NumeroMesa == comanda.NumeroMesa);
                if (mesa != null)
                    return BadRequest("mesa invalida");

                if (mesa.SituacaoMesa != 0)
                    return BadRequest("mesa ocupada");

                // alocar a nova mesa \\
                mesa.SituacaoMesa = 1;

                // desalocar a mesa atual \\
                var mesaAtual = await _context.Mesas.FirstAsync(mesa => mesa.NumeroMesa == ComandaUpdate.NumeroMesa);
                mesaAtual.SituacaoMesa = 0;

                // atualiza o numero da mesa na comanda \\
                ComandaUpdate.NumeroMesa = comanda.NumeroMesa;
            }


            if (!string.IsNullOrEmpty(comanda.NomeCliente))
                ComandaUpdate.NomeCliente = comanda.NomeCliente;

            foreach (var item in comanda.ComandaItems)
            {
                // incluir \\
                if (item.incluir)
                {
                    var novoComandaItem = new ComandaItem()
                    {
                        Comanda = ComandaUpdate,
                        CardapioItemId = item.cardapioItemId
                    };
                    await _context.ComandaItems.AddAsync(novoComandaItem);



                    // verificar se o cardapio possui preparo, se sim criar o pedido da cozinha \\
                    var cardapioItem = await _context.cardapioItems.FindAsync(item.cardapioItemId);
                    if (cardapioItem.PossuiPreparo)
                    {
                        var novoPedidoCozinha = new PedidoCozinha()
                        {
                            Comanda = ComandaUpdate,
                            SituacaoId = 1
                        };
                        await _context.PedidoCozinhas.AddAsync(novoPedidoCozinha);
                        var novoPedidoCozinhaItem = new PedidoCozinhaItem()
                        {
                            PedidoCozinha = novoPedidoCozinha,
                            ComandaItem = novoComandaItem
                        };
                        await _context.PedidoCozinhaItems.AddAsync(novoPedidoCozinhaItem);
                    };
                }

                // exluir \\
                if (item.excluir)
                {
                    var comandaItemExcluir = await _context.ComandaItems.FirstAsync(f => f.Id == item.Id);
                    _context.ComandaItems.Remove(comandaItemExcluir);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComandaExists(id))
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

        // POST: api/Comandas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comanda>> PostComanda(ComandaDto comanda)
        {
            // verificar se a mesa está disponivel \\
            // select * FROM MESAS where numeromesa = 2 \\
            var mesa = _context.Mesas.First(m => m.NumeroMesa == comanda.NumeroMesa);
            if (mesa is null)
                return BadRequest("mesa não encontrada");
            if (mesa.SituacaoMesa != 0)
            {
                return BadRequest("está mesa está ocupada");
            }
            // altera a mesa para ocupada, para não permitir abrir outra comanda para a mesma mesa \\
            mesa.SituacaoMesa = 1;

            // criando nova comanda \\
            var novaComanda = new Comanda()
            {
                NumeroMesa = comanda.NumeroMesa,
                NomeCliente = comanda.NomeCliente
            };

            // adicionando a comanda no banco \\
            // INSERT INTO comandas (id, numeromesa) VALUES(1,2) \\
            await _context.Comandas.AddAsync(novaComanda);

            foreach (var item in comanda.CardapioItems)
            {
                var novoItemComanda = new ComandaItem()
                {
                    Comanda = novaComanda,
                    CardapioItemId = item
                };

                // adicionando o novo item na comanda \\
                // INSERT INTO comandaitems (id, cardapioitemid)
                await _context.ComandaItems.AddAsync(novoItemComanda);

                // verificar se o cardapio possui preparo \\ 
                // SELECT PossuiPreparo FROM CardapioItem WHERE Id = <item> \\
                var cardapioItem = await _context.cardapioItems.FindAsync(item);
                if (cardapioItem.PossuiPreparo)
                {
                    var novoPedidoCozinha = new PedidoCozinha()
                    {
                        Comanda = novaComanda,
                        SituacaoId = 1 // PENDENTE
                    };

                    // INSERT INTO PedidoCozinha (id, comandaid, situaçãoid,  VALUES())
                    await _context.PedidoCozinhas.AddAsync(novoPedidoCozinha);

                    var novoPedidoCozinhaItem = new PedidoCozinhaItem()
                    {
                        PedidoCozinha = novoPedidoCozinha,
                        ComandaItem = novoItemComanda
                    };
                    await _context.PedidoCozinhaItems.AddAsync(novoPedidoCozinhaItem);
                }
            }

            // salvando a comanda \\
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComanda", new { id = novaComanda.Id }, comanda);
        }
    }
}
