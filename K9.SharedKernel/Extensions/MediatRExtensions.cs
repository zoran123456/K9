using K9.SharedKernel.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace K9.SharedKernel.Extensions;

public static class MediatRExtensions
{
    public static MediatRServiceConfiguration AddSharedBehaviors(this MediatRServiceConfiguration cfg)
    {
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        return cfg;
    }
}