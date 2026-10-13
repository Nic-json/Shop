namespace Shop.API.Compartilhado.Validadores;

public static class CnpjValidador
{
    public static string Normalizar(string? cnpj)
    {
        return new string((cnpj ?? "")
            .Where(c => c != '.' && c != '/' && c != '-' && c != ' ')
            .ToArray())
            .ToUpperInvariant();
    }

    public static bool EhValido(string? valor)
    {
        var cnpj = Normalizar(valor);

        if (cnpj.Length != 14)
            return false;

        // Suporta o CNPJ alfanumérico, além do numérico.
        if (cnpj.Any(c => !char.IsAsciiLetterUpper(c) &&
                          !char.IsAsciiDigit(c)))
            return false;

        if (cnpj.Distinct().Count() == 1)
            return false;

        int CalcularDigito(string baseCnpj, int[] pesos)
        {
            var soma = 0;

            for (int i = 0; i < pesos.Length; i++)
                soma += (baseCnpj[i] - 48) * pesos[i];

            var resto = soma % 11;

            return resto < 2 ? 0 : 11 - resto;
        }

        int[] pesosPrimeiro =
        {
            5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2
        };

        int[] pesosSegundo =
        {
            6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2
        };

        var primeiro = CalcularDigito(cnpj[..12], pesosPrimeiro);
        var segundo = CalcularDigito(cnpj[..13], pesosSegundo);

        return cnpj[12] == (char)('0' + primeiro)
            && cnpj[13] == (char)('0' + segundo);
    }
}