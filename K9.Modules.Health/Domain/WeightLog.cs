using K9.SharedKernel.Domain;

namespace K9.Modules.Health.Domain;

public class WeightLog : Entity
{
    public decimal Weight { get; private set; }
    public DateTime MeasuredAt { get; private set; }

    private WeightLog() { }

    internal WeightLog(Guid id, decimal weight, DateTime measuredAt) : base(id)
    {
        if (weight <= 0)
            throw new ArgumentException("Weight must be positive.");

        Weight = weight;
        MeasuredAt = measuredAt;
    }
}