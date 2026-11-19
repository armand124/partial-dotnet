using System.Reflection;

namespace RoomHub.Api.Common.Cqrs;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        Type[] handlerContracts = [typeof(ICommandHandler<,>)];

        var handlerTypes = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var implementation in handlerTypes)
        {
            foreach (var contract in implementation.GetInterfaces())
            {
                if (contract.IsGenericType
                    && handlerContracts.Contains(contract.GetGenericTypeDefinition()))
                {
                    services.AddScoped(contract, implementation);
                }
            }
        }

        return services;
    }
}
