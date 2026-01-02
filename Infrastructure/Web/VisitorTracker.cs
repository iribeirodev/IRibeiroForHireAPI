using System.Security.Cryptography;
using System.Text;

namespace IRibeiroForHireAPI.Infrastructure.Web;

/// <summary>
/// Componente de rastreamento de visitantes anônimos.
/// </summary>
public class VisitorTracker(IHttpContextAccessor http)
{
    private const string VisitorCookieName = "visitor_token";

    /// <summary>
    /// Recupera o identificador do visitante do cookie ou gera um novo caso não exista.
    /// </summary>
    /// <returns>Uma string hash única representando o navegador do visitante.</returns>
    /// <exception cref="InvalidOperationException">Lançada se tentado acessar fora de um contexto HTTP.</exception>
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
            SameSite = SameSiteMode.Lax
        });

        return hashed;
    }
}
