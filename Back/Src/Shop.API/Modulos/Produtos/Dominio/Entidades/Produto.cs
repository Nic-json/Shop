using Shop.API.Modulos.Produtos.Dominio.Enums;

namespace Shop.API.Modulos.Produtos.Dominio.Entidades;

public class Produto
{
    public Guid Id { get; private set; }

    public Guid EmpresaId { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public string CodigoInterno { get; private set; } = string.Empty;

    public string? CodigoBarras { get; private set; }

    public UnidadeMedida UnidadeMedida { get; private set; }

    public decimal PrecoCusto { get; private set; }

    public decimal PrecoVenda { get; private set; }

    public bool Ativo { get; private set; }
}