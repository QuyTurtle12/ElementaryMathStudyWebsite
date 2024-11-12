using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.CheckoutPages
{
    public class AddItemsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public AddItemsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public CartCreateDto? CartItem { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("api/cart", CartItem);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("ViewCart");
            }

            ModelState.AddModelError(string.Empty, "Error adding items to cart.");
            return Page();
        }
    }
}
