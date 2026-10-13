namespace Shop.API.Modulos.Lojas.Aplicacao.DTOs;

public class AtualizarLojaRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
}