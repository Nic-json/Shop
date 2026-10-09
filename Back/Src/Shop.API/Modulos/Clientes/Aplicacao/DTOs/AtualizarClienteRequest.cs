namespace Shop.API.Modulos.Clientes.Aplicacao.DTOs;

public class AtualizarClienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
}