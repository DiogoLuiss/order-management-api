using FluentValidation;

namespace OrderManagementApi.Service
{
    public interface IValidationService
    {
        Task<(bool IsValid, object? ErrorResponse)> ValidateAsync<T>(T viewModel, IValidator<T> validator) where T : class;
    }

    public class ValidationService : IValidationService
    {
        public async Task<(bool IsValid, object? ErrorResponse)> ValidateAsync<T>(T viewModel, IValidator<T> validator) where T : class
        {
            if (viewModel == null)
                return (false, new { Message = "Os dados fornecidos estão inválidos." });

            var resultado = await validator.ValidateAsync(viewModel);

            if (!resultado.IsValid)
            {
                return (false, new
                {
                    Message = resultado.Errors.Select(e => e.ErrorMessage).ToArray()
                });
            }

            return (true, null);
        }
    }
}
