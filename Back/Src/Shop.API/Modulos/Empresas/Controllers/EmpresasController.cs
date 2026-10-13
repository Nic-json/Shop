
using Microsoft.AspNetCore.Mvc;
using Shop.API.Modulos.Empresas.Aplicacao.DTOs;
using Shop.API.Modulos.Empresas.Aplicacao.Servicos;

namespace Shop.API.Modulos.Empresas.Controllers;

[ApiController]
[Route("api/empresas")]
public class EmpresasController : ControllerBase
{
    private readonly EmpresaService _empresaService;

    public EmpresasController(EmpresaService empresaService)
    {
        _empresaService = empresaService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromBody] CriarEmpresaRequest request)
    {
        try
        {
            var id = await _empresaService.CriarAsync(request);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id },
                new { Id = id, Mensagem = "Empresa cadastrada com sucesso." }
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var empresas = await _empresaService.ListarAsync();

        return Ok(empresas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var empresa = await _empresaService.BuscarPorIdAsync(id);

        if (empresa == null)
            return NotFound(new { Mensagem = "Empresa não encontrada." });

        return Ok(empresa);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        [FromBody] AtualizarEmpresaRequest request)
    {
        try
        {
            var atualizado = await _empresaService.AtualizarAsync(id, request);

            if (!atualizado)
                return NotFound(new { Mensagem = "Empresa não encontrada." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Mensagem = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Desativar(Guid id)
    {
        var desativado = await _empresaService.DesativarAsync(id);

        if (!desativado)
            return NotFound(new { Mensagem = "Empresa não encontrada ou já desativada." });

        return NoContent();
    }
}
