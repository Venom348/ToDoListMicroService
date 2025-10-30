using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Core.Options;

/// <summary>
///     Класс для настройки генерации JWT-токена
/// </summary>
public class AuthOption
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    
    public SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
}