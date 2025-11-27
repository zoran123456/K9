namespace K9.Modules.Identity.Services;

public class MockGoogleAuthService : IGoogleAuthService
{
    public Task<GoogleUserInfo> ValidateTokenAsync(string idToken)
    {
        if (idToken.StartsWith("valid_", StringComparison.Ordinal))
        {
            return Task.FromResult(new GoogleUserInfo(
                SubjectId: "google_123456789",
                Email: "zoran@test.com",
                FirstName: "Zoran",
                LastName: "K9 Architect"));
        }

        throw new UnauthorizedAccessException("Google token is invalid.");
    }
}