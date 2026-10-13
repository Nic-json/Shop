namespace Shop.API.Modulos.Lojas.Aplicacao.DTOs;

public class LojaResponse
{
    public Guid Id { get; set; }
    public Guid EmpresaId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }
    public bool Ativo { get; set; }
}