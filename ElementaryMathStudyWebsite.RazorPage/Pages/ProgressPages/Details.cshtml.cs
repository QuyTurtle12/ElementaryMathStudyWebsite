using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ProgressPages
{
    public class DetailsModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public DetailsModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public Progress Progress { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var progress = await _context.Progress.FirstOrDefaultAsync(m => m.StudentId == id);
            if (progress == null)
            {
                return NotFound();
            }
            else
            {
                Progress = progress;
            }
            return Page();
        }
    }
}
