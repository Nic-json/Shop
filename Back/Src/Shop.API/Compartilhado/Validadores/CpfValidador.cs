
namespace Shop.API.Compartilhado.Validadores;

public static class CpfValidador
{
    public static string Normalizar(string? cpf)
    {
        return new string((cpf ?? "")
            .Where(char.IsDigit)
            .ToArray());
    }

    public static bool EhValido(string? valor)
    {
        var cpf = Normalizar(valor);

        if (cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        int CalcularDigito(string numeros, int pesoInicial)
        {
            var soma = 0;

            for (int i = 0; i < numeros.Length; i++)
            {
                soma += (numeros[i] - '0') * (pesoInicial - i);
            }

            var resto = soma % 11;

            return resto < 2 ? 0 : 11 - resto;
        }

        var primeiroDigito = CalcularDigito(cpf[..9], 10);
        var segundoDigito = CalcularDigito(cpf[..10], 11);

        return cpf[9] == (char)('0' + primeiroDigito)
            && cpf[10] == (char)('0' + segundoDigito);
    }
}
