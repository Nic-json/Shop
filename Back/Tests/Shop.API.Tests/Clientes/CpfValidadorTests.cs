
using Shop.API.Compartilhado.Validadores;

namespace Shop.API.Tests.Clientes;

public class CpfValidadorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("16899535009")]
    public void EhValido_QuandoCpfValido_DeveRetornarTrue(
        string cpf)
    {
        // Act
        var resultado = CpfValidador.EhValido(cpf);

        // Assert
        Assert.True(resultado);
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("12345678900")]
    [InlineData("123")]
    public void EhValido_QuandoCpfInvalido_DeveRetornarFalse(
        string cpf)
    {
        // Act
        var resultado = CpfValidador.EhValido(cpf);

        // Assert
        Assert.False(resultado);
    }

    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("168.995.350-09", "16899535009")]
    public void Normalizar_QuandoCpfFormatado_DeveRemoverPontuacao(
        string cpf,
        string esperado)
    {
        // Act
        var resultado = CpfValidador.Normalizar(cpf);

        // Assert
        Assert.Equal(esperado, resultado);
    }
}
