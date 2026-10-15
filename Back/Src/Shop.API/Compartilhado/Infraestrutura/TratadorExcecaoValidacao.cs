
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shop.API.Compartilhado.Validacao;

namespace Shop.API.Compartilhado.Infraestrutura;

public class TratadorExcecaoValidacao : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ExcecaoValidacao erroValidacao)
            return false;

        var resposta = new ValidationProblemDetails(erroValidacao.Erros)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Ocorreram erros de validação."
        };

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(
            resposta,
            cancellationToken
        );

        return true;
    }
}
