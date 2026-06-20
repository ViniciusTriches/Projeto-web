namespace Marketplace.Web.MVC.Models.ApiContracts.Usuarios;

public enum EUsuarioFuncao : short
{
    Comprador = 0,
    Vendedor = 1,
    Administrador = 2
}

public enum EUsuarioStatus : short
{
    Ativo = 0,
    Inativo = 1,
    Suspenso = 2
}

public enum ETipoPessoa : short
{
    PessoaFisica = 0,
    PessoaJuridica = 1
}

public record LoginRequest(string Email, string Senha);

public record CriarUsuarioRequest(
    string Nome,
    string Email,
    string Senha,
    string Documento,
    ETipoPessoa TipoPessoa,
    EUsuarioFuncao Funcao = EUsuarioFuncao.Comprador,
    string? NomeFantasia = null,
    DateOnly? DataNascimento = null,
    string? Telefone = null);

public record AtualizarUsuarioRequest(Guid Id, string Nome, string Email);

public record RefreshTokenRequest(string AccessToken, string RefreshToken);

public record EsqueciSenhaRequest(string Email);

public record ResetarSenhaRequest(string Token, string NovaSenha);

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresIn,
    Guid UsuarioId,
    string Nome,
    string Email);

public record TokenDto(string AccessToken, string RefreshToken, DateTime ExpiresIn);

public record UsuarioDto(
    Guid Id,
    string Nome,
    string? NomeFantasia,
    string Email,
    string Documento,
    DateOnly? DataNascimento,
    string? Telefone,
    ETipoPessoa TipoPessoa,
    EUsuarioFuncao Funcao,
    EUsuarioStatus Status,
    DateTime CriadoEm);
