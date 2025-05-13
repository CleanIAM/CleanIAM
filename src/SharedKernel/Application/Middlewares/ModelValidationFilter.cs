using System.Net;
using JasperFx.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.Infrastructure.Utils;

namespace SharedKernel.Application.Middlewares;

/// <summary>
/// Middleware to validate model state and return eventual errors in correct format.
/// </summary>
/// <remarks>
/// Only applied on API requests.
/// </remarks>
public class ModelValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Handle model error only for API requests
            var isApiRequest = context.HttpContext.Request.Path.Value?.ToLowerInvariant().Contains("/api/") ?? false;
            
            if (isApiRequest && !context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value is { Errors.Count: > 0 })
                    .Select(e => 
                     e.Value?.Errors.Select(x => x.ErrorMessage).Join(", ")
                    ).Where(s => !string.IsNullOrEmpty(s))!.Join(", ");

                context.Result = Result.Error(errors, HttpStatusCode.BadRequest);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing after the action executes
        }
}