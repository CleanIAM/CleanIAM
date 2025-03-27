using Identity.Api.ViewModels.Signup;
using Identity.Application.Commands.Users;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Infrastructure;
using Wolverine;

namespace Identity.Api.Controllers;

[Route("/")]
public class SignupController(IMessageBus bus) : Controller
{
    [HttpGet("signup")]
    public IActionResult Signup()
    {
        return View();
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromForm] SignupViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Create user
        var command = model.Adapt<CreateNewUserCommand>();
        var res = await bus.InvokeAsync<Result>(command, cancellationToken);

        if (res.IsError())
        {
            ModelState.AddModelError("email", res.ErrorValue.Message);
            return View(model);
        }

        // Send email verification


        return View("UserRegistered", new UserRegisteredViewModel
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        });
    }
}