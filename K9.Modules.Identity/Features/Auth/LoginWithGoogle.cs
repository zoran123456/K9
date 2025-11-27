using FluentValidation;
using K9.Modules.Identity.Domain;
using K9.Modules.Identity.Persistence;
using K9.Modules.Identity.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace K9.Modules.Identity.Features.Auth;

public record LoginWithGoogleCommand(string IdToken) : IRequest<LoginResult>;

public record LoginResult(Guid UserId, string Email, string FirstName, string Token, bool IsNewUser);

public class LoginWithGoogleValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleValidator()
    {
        RuleFor(x => x.IdToken).NotEmpty().WithMessage("Google ID Token is required.");
    }
}

public class LoginWithGoogleHandler : IRequestHandler<LoginWithGoogleCommand, LoginResult>
{
    private readonly IdentityDbContext _context;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly ITokenService _tokenService;

    public LoginWithGoogleHandler(
        IdentityDbContext context,
        IGoogleAuthService googleAuthService,
        ITokenService tokenService)
    {
        _context = context;
        _googleAuthService = googleAuthService;
        _tokenService = tokenService;
    }

    public async Task<LoginResult> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        var googleUser = await _googleAuthService.ValidateTokenAsync(request.IdToken);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleSubjectId == googleUser.SubjectId, cancellationToken);

        var isNewUser = false;

        if (user == null)
        {
            isNewUser = true;
            user = new ApplicationUser(
                Guid.NewGuid(),
                googleUser.Email,
                googleUser.FirstName,
                googleUser.LastName,
                googleUser.SubjectId);

            _context.Users.Add(user);
        }
        else
        {
            user.UpdateName(googleUser.FirstName, googleUser.LastName);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateJwt(user);

        return new LoginResult(user.Id, user.Email, user.FirstName, token, isNewUser);
    }
}