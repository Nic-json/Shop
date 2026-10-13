
namespace Shop.API.Modulos.Lojas.Dominio.Entidades;

public class Loja
{
    public Guid Id { get; private set; }
    public Guid EmpresaId { get; private set; }

    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? Endereco { get; set; }

    public DateTime CriadoEm { get; private set; }
    public bool Ativo { get; set; }

    private Loja() { }

    public Loja(
        Guid empresaId,
        string nome,
        string? telefone,
        string? endereco)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Nome = nome;
        Telefone = telefone;
        Endereco = endereco;
        CriadoEm = DateTime.UtcNow;
        Ativo = true;
    }
}
