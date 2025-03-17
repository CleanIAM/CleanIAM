using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Identity.Api.Controllers;

[Route("/console")]
public class ClientController(IOpenIddictApplicationManager applicationManager): Controller
{
    [HttpGet("clients")]
    public async Task<IActionResult> Clients()
    {
        var clients = applicationManager.ListAsync();
        await foreach (var client in clients)
        {
            System.Console.WriteLine("Client: {client.ClientId}");
        }
        
        return new OkObjectResult(clients);
    }
}