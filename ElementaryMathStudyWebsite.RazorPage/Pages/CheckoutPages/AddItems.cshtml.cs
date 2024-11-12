using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.CheckoutPages
{
    public class AddItemsModel : PageModel
    {

        public AddItemsModel()
        {
        }

        [BindProperty]
        public CartCreateDto? CartItem { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            return Page();
        }
    }
}
