namespace Shop.API.Modulos.Clientes.Dominio.Entidades;

public class Cliente
{
    public Guid Id { get; private set; }
    public Guid EmpresaId { get; private set; }

    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    public DateTime CriadoEm { get; private set; }
    public bool Ativo { get; set; }

    private Cliente() { }

    public Cliente(
        Guid empresaId,
        string nome,
        string? cpfCnpj,
        string? email,
        string? telefone)
    {
        Id = Guid.NewGuid();
        EmpresaId = empresaId;
        Nome = nome;
        CpfCnpj = cpfCnpj;
        Email = email;
        Telefone = telefone;
        CriadoEm = DateTime.UtcNow;
        Ativo = true;
    }
}