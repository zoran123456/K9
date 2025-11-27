using FluentValidation;
using K9.Modules.Health.Domain;
using K9.Modules.Health.Persistence;
using MediatR;

namespace K9.Modules.Health.Features.Dogs;

public record RegisterDogCommand(
    string Name,
    string Breed,
    DateTime DateOfBirth,
    Guid OwnerId
) : IRequest<Guid>;

public class RegisterDogValidator : AbstractValidator<RegisterDogCommand>
{
    public RegisterDogValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Breed)
            .NotEmpty().WithMessage("Breed is required.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.OwnerId)
            .NotEmpty();
    }
}

public class RegisterDogHandler : IRequestHandler<RegisterDogCommand, Guid>
{
    private readonly HealthDbContext _context;

    public RegisterDogHandler(HealthDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegisterDogCommand request, CancellationToken cancellationToken)
    {
        var dog = new DogProfile(
            Guid.NewGuid(),
            request.Name,
            request.DateOfBirth,
            request.Breed,
            request.OwnerId
        );

        _context.DogProfiles.Add(dog);
        await _context.SaveChangesAsync(cancellationToken);

        return dog.Id;
    }
}