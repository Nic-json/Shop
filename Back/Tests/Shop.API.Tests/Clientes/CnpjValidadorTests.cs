
using Shop.API.Compartilhado.Validadores;

namespace Shop.API.Tests.Clientes;

public class CnpjValidadorTests
{
    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11444777000161")]
    public void EhValido_QuandoCnpjValido_DeveRetornarTrue(
        string cnpj)
    {
        var resultado = CnpjValidador.EhValido(cnpj);

        Assert.True(resultado);
    }

    [Theory]
    [InlineData("11111111111111")]
    [InlineData("12345678000100")]
    [InlineData("123")]
    public void EhValido_QuandoCnpjInvalido_DeveRetornarFalse(
        string cnpj)
    {
        var resultado = CnpjValidador.EhValido(cnpj);

        Assert.False(resultado);
    }

    [Theory]
    [InlineData("11.222.333/0001-81", "11222333000181")]
    [InlineData("11.444.777/0001-61", "11444777000161")]
    public void Normalizar_QuandoCnpjFormatado_DeveRemoverPontuacao(
        string cnpj,
        string esperado)
    {
        var resultado = CnpjValidador.Normalizar(cnpj);

        Assert.Equal(esperado, resultado);
    }
}
