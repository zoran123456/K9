namespace K9.Modules.Identity.Services;
public record GoogleUserInfo(string SubjectId, string Email, string FirstName, string LastName);

public interface IGoogleAuthService
{
    Task<GoogleUserInfo> ValidateTokenAsync(string idToken);
}