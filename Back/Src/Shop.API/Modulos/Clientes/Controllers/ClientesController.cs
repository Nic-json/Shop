
using Microsoft.AspNetCore.Mvc;
using Shop.API.Modulos.Clientes.Aplicacao.DTOs;
using Shop.API.Modulos.Clientes.Aplicacao.Servicos;

namespace Shop.API.Modulos.Clientes.Controllers;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;

    public ClientesController(ClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarClienteRequest request)
    {
        // Empresa temporária para testes locais
        var empresaId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        var clienteId = await _clienteService.CriarAsync(
            request,
            empresaId
        );

        return StatusCode(201, new
        {
            Id = clienteId,
            Mensagem = "Cliente cadastrado com sucesso."
        });
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var empresaId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        var clientes = await _clienteService.ListarAsync(empresaId);

        return Ok(clientes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var empresaId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        var cliente = await _clienteService.BuscarPorIdAsync(
            id,
            empresaId
        );

        if (cliente == null)
        {
            return NotFound(new
            {
                Mensagem = "Cliente não encontrado."
            });
        }

        return Ok(cliente);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarClienteRequest request)
    {
        // Empresa temporária para testes locais
        var empresaId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        var atualizado = await _clienteService.AtualizarAsync(
            id,
            empresaId,
            request
        );

        if (!atualizado)
        {
            return NotFound(new
            {
                Mensagem = "Cliente não encontrado."
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var empresaId = Guid.Parse(
            "11111111-1111-1111-1111-111111111111"
        );

        var desativado = await _clienteService.DesativarAsync(
            id,
            empresaId
        );

        if (!desativado)
        {
            return NotFound(new
            {
                Mensagem = "Cliente não encontrado ou já desativado."
            });
        }

        return NoContent();
    }
}
