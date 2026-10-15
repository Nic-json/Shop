
namespace Shop.API.Compartilhado.Validacao;

public class ExcecaoValidacao : Exception
{
    public Dictionary<string, string[]> Erros { get; }

    public ExcecaoValidacao(Dictionary<string, string[]> erros)
        : base("Ocorreram erros de validação.")
    {
        Erros = erros;
    }
}
