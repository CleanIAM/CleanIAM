using System.ComponentModel.DataAnnotations;
using Mapster;
using ManagementPortal.Application.Commands;
using ManagementPortal.Application.Queries.OpenIdClients;
using ManagementPortal.Core;
using ManagementPortal.Core.OpenIdApplication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenIddict.Abstractions;
using SharedKernel.Infrastructure;
using Wolverine;

namespace ManagementPortal.Api.Views.Clients;

public class EditModel : PageModel
{
    private readonly IMessageBus _bus;

    public EditModel(IMessageBus bus)
    {
        _bus = bus;
    }

    [BindProperty]
    public OpenIdApplication Client { get; set; }

    [TempData]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Valid Client ID is required");
        }

        var query = new GetOpenIdApplicationByIdQuery(id);
        var application = await _bus.InvokeAsync<OpenIdApplication?>(query);
        
        if (application is null)
        {
            return NotFound();
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Use Mapster to map from view model to domain model
            var application = Client.Adapt<OpenIdApplication>();
            
            // Update the application using the command
            var command = new UpdateOpenIdApplicationCommand(application);
            var result = await _bus.InvokeAsync<Result>(command);

            if (result.IsError())
            {
                ModelState.AddModelError(string.Empty, result.ErrorValue.Message);
                return Page();
            }

            StatusMessage = "Client updated successfully";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
            return Page();
        }
    }

}