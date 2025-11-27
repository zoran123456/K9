using FluentValidation;
using K9.Modules.Health.Persistence;
using MediatR;

namespace K9.Modules.Health.Features.Vaccinations;

public record RecordVaccinationCommand(
    Guid DogId,
    string VaccineName,
    DateTime DateAdministered,
    int ValidityInMonths,
    string VetClinicName
) : IRequest;

public class RecordVaccinationValidator : AbstractValidator<RecordVaccinationCommand>
{
    public RecordVaccinationValidator()
    {
        RuleFor(x => x.DogId).NotEmpty();

        RuleFor(x => x.VaccineName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ValidityInMonths)
            .GreaterThan(0).WithMessage("Vaccine must be valid for at least 1 month.");

        RuleFor(x => x.DateAdministered)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Vaccination date cannot be in the future.");
    }
}

public class RecordVaccinationHandler : IRequestHandler<RecordVaccinationCommand>
{
    private readonly HealthDbContext _context;

    public RecordVaccinationHandler(HealthDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RecordVaccinationCommand request, CancellationToken cancellationToken)
    {
        var dog = await _context.DogProfiles.FindAsync([request.DogId], cancellationToken);

        if (dog == null)
        {
            throw new ArgumentException($"Dog with ID {request.DogId} was not found.", nameof(request));
        }

        dog.AddVaccination(
            request.VaccineName,
            request.DateAdministered,
            request.ValidityInMonths,
            request.VetClinicName
        );

        await _context.SaveChangesAsync(cancellationToken);
    }
}