
using Microsoft.AspNetCore.Mvc;
using Shop.API.Modulos.Lojas.Aplicacao.DTOs;
using Shop.API.Modulos.Lojas.Aplicacao.Servicos;

namespace Shop.API.Modulos.Lojas.Controllers;

[ApiController]
[Route("api/lojas")]
public class LojasController : ControllerBase
{
    private readonly LojaService _lojaService;

    // Temporário: somente para desenvolvimento local.
    // Deve ser substituído pelo EmpresaId autenticado.
    private static readonly Guid EmpresaId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public LojasController(LojaService lojaService)
    {
        _lojaService = lojaService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarLojaRequest request)
    {
        try
        {
            var id = await _lojaService.CriarAsync(EmpresaId, request);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id },
                new { Id = id, Mensagem = "Loja cadastrada com sucesso." }
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Mensagem = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lojas = await _lojaService.ListarAsync(EmpresaId);

        return Ok(lojas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var loja = await _lojaService.BuscarPorIdAsync(id, EmpresaId);

        if (loja == null)
            return NotFound(new { Mensagem = "Loja não encontrada." });

        return Ok(loja);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarLojaRequest request)
    {
        try
        {
            var atualizado = await _lojaService.AtualizarAsync(
                id, EmpresaId, request);

            if (!atualizado)
                return NotFound(new { Mensagem = "Loja não encontrada." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var desativado = await _lojaService.DesativarAsync(
            id, EmpresaId);

        if (!desativado)
            return NotFound(new { Mensagem = "Loja não encontrada." });

        return NoContent();
    }
}
