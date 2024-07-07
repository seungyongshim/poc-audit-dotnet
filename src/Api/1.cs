using FluentValidation.AspNetCore.Http;
using FluentValidation.Results;

namespace Api
{
    public class SimpleResultsFactory : IFluentValidationEndpointFilterResultsFactory
    {
        public IResult Create(ValidationResult validationResult)
        {
            Dictionary<string, string[]> errors = [];
            var errorByProperty = validationResult.Errors.GroupBy(x => x.PropertyName);
            foreach (var error in errorByProperty)
            {
                errors.Add(error.Key, error.Select(x => x.ErrorMessage).ToArray());
            }
            return TypedResults.ValidationProblem(errors);
        }
    }
}
