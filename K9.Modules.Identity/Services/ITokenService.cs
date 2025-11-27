using K9.Modules.Identity.Domain;

namespace K9.Modules.Identity.Services;

public interface ITokenService
{
    string GenerateJwt(ApplicationUser user);
}