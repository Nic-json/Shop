namespace Shop.API.Modulos.Empresas.Aplicacao.DTOs;

public class CriarEmpresaRequest
{
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
}