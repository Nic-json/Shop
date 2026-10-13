namespace Shop.API.Modulos.Empresas.Dominio.Entidades;

public class Empresa
{
    public Guid Id { get; private set; }

    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;

    public DateTime CriadoEm { get; private set; }
    public bool Ativo { get; set; }

    private Empresa() { }

    public Empresa(
        string razaoSocial,
        string nomeFantasia,
        string cnpj)
    {
        Id = Guid.NewGuid();
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Cnpj = cnpj;
        CriadoEm = DateTime.UtcNow;
        Ativo = true;
    }
}