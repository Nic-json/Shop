
namespace Shop.API.Compartilhado.Validacao;

public class ResultadoValidacao
{
    private readonly Dictionary<string, List<string>> _erros = new();

    public bool EhValido => _erros.Count == 0;

    public void AdicionarErro(string campo, string mensagem)
    {
        if (!_erros.TryGetValue(campo, out var mensagens))
        {
            mensagens = new List<string>();
            _erros[campo] = mensagens;
        }

        mensagens.Add(mensagem);
    }

    public Dictionary<string, string[]> ObterErros()
    {
        return _erros.ToDictionary(
            item => item.Key,
            item => item.Value.ToArray()
        );
    }

    public void LancarSeInvalido()
    {
        if (!EhValido)
        {
            throw new ExcecaoValidacao(ObterErros());
        }
    }
}
