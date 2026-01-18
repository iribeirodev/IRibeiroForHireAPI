using System.Security.Cryptography;
using System.Text;

namespace IRibeiroForHireAPI.Infrastructure.Web;

/// <summary>
/// Componente responsável pelo rastreamento e identificação de visitantes anônimos através de cookies persistentes.
/// </summary>
/// <remarks>
/// Este serviço utiliza um cookie seguro para manter a identidade do visitante entre sessões, 
/// permitindo o controle de histórico e aplicação de regras de Rate Limit.
/// </remarks>
/// <param name="http">Acessor para o contexto HTTP atual da requisição.</param>
public class VisitorTracker(IHttpContextAccessor http)
{
    private const string VisitorCookieName = "visitor_token";

    /// <summary>
    /// Recupera o identificador único do visitante do cookie de rastreamento ou gera um novo token caso ele não exista.
    /// </summary>
    /// <remarks>
    /// Ao gerar um novo ID, um cookie seguro é anexado à resposta HTTP com validade de 3 anos.
    /// O token é gerado através de um hash SHA256 de um GUID para garantir unicidade e anonimato.
    /// </remarks>
    /// <returns>Uma string em formato hexadecimal representando o identificador único do visitante.</returns>
    /// <exception cref="InvalidOperationException">Lançada caso o método seja chamado fora do ciclo de vida de uma requisição HTTP.</exception>
    public string GetOrCreateVisitorId()
    {
        var context = http.HttpContext ?? throw new InvalidOperationException("HTTP Context indisponível.");
        
        // Recupera o ID existente vindo do navegador do cliente
        if (context.Request.Cookies.TryGetValue(VisitorCookieName, out var existing))
            return existing;

        // Caso não exista, gera um novo identificador único
        // Usando SHA256 sobre um GUID para gerar um token fixo
        string hashed = Convert.ToHexString(
                        SHA256.HashData(
                            Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())));

        context.Response.Cookies.Append(VisitorCookieName, hashed, new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddYears(3),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        return hashed;
    }
}
