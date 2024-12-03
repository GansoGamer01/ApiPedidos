using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiPedidos.BancoDeDados;
using ApiPedidos.Modelos;
using ApiPedidos.DTOs;

namespace ApiPedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly PedidoContexto _context;

        public ClientesController(PedidoContexto context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = _context.Clientes
        .Select(c => new ClienteDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Email = c.Email,
            Telefone = c.Telefone,
            Endereco = c.Endereco,
            Cidade = c.Cidade,
            Estado = c.Estado,
            Cep = c.Cep,
            DataCadastro = c.DataCadastro
        })
        .ToList();
            return Ok(clientes);
        }

        // GET: api/Clientes/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetClienteById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<IActionResult> PostCliente(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClienteById), new { id = cliente.Id }, cliente);
        }

        // PUT: api/Clientes/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente clienteAtualizado)
        {
            if (id != clienteAtualizado.Id)
            {
                return BadRequest("O ID do cliente não corresponde ao parâmetro da URL.");
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            cliente.Nome = clienteAtualizado.Nome;
            cliente.Email = clienteAtualizado.Email;
            cliente.Telefone = clienteAtualizado.Telefone;
            cliente.Endereco = clienteAtualizado.Endereco;
            cliente.Cidade = clienteAtualizado.Cidade;
            cliente.Estado = clienteAtualizado.Estado;
            cliente.Cep = clienteAtualizado.Cep;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
