using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

namespace PROJE_UI.Validation
{
    public class FluentValidationExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is FluentValidation.ValidationException validationException)
            {
                var errors = validationException.Errors.Select(e => new ValidationError
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage
                }).ToList();
                var tempData = context.HttpContext.Items["TempData"] as TempDataDictionary;
                tempData["FluentValidationErrors"] = errors;
                context.Result = new RedirectToActionResult("Login", "User", null);

                context.ExceptionHandled = true;
            }
        }

        public class ValidationError
        {
            public string PropertyName { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
