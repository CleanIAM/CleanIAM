using Identity.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Identity.Api.Controllers;

[Route("/console")]
public class ClientController(IOpenIddictApplicationManager applicationManager, IMailService mailService) : Controller
{
    [HttpGet("clients")]
    public async Task<IActionResult> Clients()
    {
        var clients = applicationManager.ListAsync();
        await foreach (var client in clients)
            System.Console.WriteLine("Client: {client.ClientId}");

        return new OkObjectResult(clients);
    }

    [HttpGet("/email/test")]
    public async Task<IActionResult> EmailTest()
    {
        var res = await mailService.SendVerificationEmailAsync(
            new() { Email = "a@a.a", Name = "X Y" }, Guid.Empty);
        return res;
    }
}