using FluentValidation;
using System.Reflection;

namespace OrderManagementApi.Service
{
    public static class ValidatorRegistrationService
    {
        public static void AddValidators(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= Assembly.GetExecutingAssembly();

            var validatorTypes = assembly.GetTypes()
                .Where(t => t.BaseType != null &&
                            t.BaseType.IsGenericType &&
                            t.BaseType.GetGenericTypeDefinition() == typeof(AbstractValidator<>));

            foreach (var type in validatorTypes)
            {
                services.AddScoped(type);
            }
        }
    }
}
