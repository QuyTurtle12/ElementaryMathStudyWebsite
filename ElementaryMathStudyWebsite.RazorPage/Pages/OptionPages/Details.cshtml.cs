using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OptionPages
{
    public class DetailsModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public DetailsModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Option Option { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Option.FirstOrDefaultAsync(m => m.Id == id);

            if (option == null)
            {
                return NotFound();
            }
            else
            {
                Option = option;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Option.FindAsync(id);
            if (option != null)
            {
                Option = option;
                _context.Option.Remove(Option);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
