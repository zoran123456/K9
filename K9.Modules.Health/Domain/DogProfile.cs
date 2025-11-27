using K9.Modules.Health.Domain.Events;
using K9.SharedKernel.Domain;

namespace K9.Modules.Health.Domain;

public class DogProfile : Entity
{
    public string Name { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string Breed { get; private set; }
    public Guid OwnerId { get; private set; }

    private readonly List<VaccinationRecord> _vaccinations = new();
    public IReadOnlyCollection<VaccinationRecord> Vaccinations => _vaccinations.AsReadOnly();

    private readonly List<WeightLog> _weightLogs = new();
    public IReadOnlyCollection<WeightLog> WeightLogs => _weightLogs.AsReadOnly();

    public DogProfile(Guid id, string name, DateTime dateOfBirth, string breed, Guid ownerId) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        Name = name;
        DateOfBirth = dateOfBirth;
        Breed = breed;
        OwnerId = ownerId;
    }

    public void AddVaccination(string vaccineName, DateTime administeredDate, int validityInMonths, string clinicName)
    {
        var record = new VaccinationRecord(Guid.NewGuid(), vaccineName, administeredDate, validityInMonths, clinicName);
        _vaccinations.Add(record);
    }

    public void LogWeight(decimal newWeight, DateTime measuredAt)
    {
        if (newWeight <= 0) throw new ArgumentException("Weight must be greater than zero.", nameof(newWeight));
        CheckWeightTrend(newWeight, measuredAt);
        var log = new WeightLog(Guid.NewGuid(), newWeight, measuredAt);
        _weightLogs.Add(log);
    }

    private void CheckWeightTrend(decimal newWeight, DateTime currentMeasurementDate)
    {
        var thresholdDate = currentMeasurementDate.AddDays(-30);

        var lastMeasurement = _weightLogs
            .Where(w => w.MeasuredAt >= thresholdDate)
            .OrderByDescending(w => w.MeasuredAt)
            .FirstOrDefault();

        if (lastMeasurement == null) return;

        var weightDifference = Math.Abs(newWeight - lastMeasurement.Weight);
        var percentageChange = (weightDifference / lastMeasurement.Weight) * 100;

        if (percentageChange <= 10) return;

        var changeType = newWeight > lastMeasurement.Weight ? "increased" : "decreased";

        AddDomainEvent(new WeightAlertEvent(
            Id,
            newWeight,
            lastMeasurement.Weight,
            $"Warning: Weight has {changeType} sharply by {percentageChange:F1}% (from {lastMeasurement.Weight}kg to {newWeight}kg) compared to measurement on {lastMeasurement.MeasuredAt:d}."
        ));
    }
}