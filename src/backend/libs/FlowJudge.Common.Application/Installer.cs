using FlowJudge.Common.Application.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace FlowJudge.Common.Application
{
    public static class Installer
    {
        public static IServiceCollection AddMediator(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(assemblies);

            services.TryAddScoped<IMediator, MediatorImpl>();

            var uniqueAssemblies = assemblies
                .Where(static assembly => assembly is not null)
                .Distinct()
                .ToArray();

            foreach (var assembly in uniqueAssemblies)
            {
                RegisterHandlersFromAssembly(services, assembly);
            }

            return services;
        }

        private static void RegisterHandlersFromAssembly(
            IServiceCollection services,
            Assembly assembly)
        {
            var implementationTypes = assembly
                .GetTypes()
                .Where(static type =>
                    type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
                .ToArray();

            foreach (var implementationType in implementationTypes)
            {
                var handlerInterfaces = implementationType
                    .GetInterfaces()
                    .Where(static i =>
                        i.IsGenericType &&
                        (
                            i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                            i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)
                        ))
                    .ToArray();

                foreach (var handlerInterface in handlerInterfaces)
                {
                    services.TryAdd(new ServiceDescriptor(
                        handlerInterface,
                        implementationType,
                        ServiceLifetime.Scoped));
                }
            }
        }
    }
}
