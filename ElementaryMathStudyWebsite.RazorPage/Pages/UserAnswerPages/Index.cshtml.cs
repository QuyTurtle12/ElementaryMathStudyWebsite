using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    public class IndexModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public IndexModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IList<UserAnswer> UserAnswer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            UserAnswer = await _context.UserAnswer
                .Include(u => u.Option)
                .Include(u => u.Question)
                .Include(u => u.User).ToListAsync();
        }
    }
}
