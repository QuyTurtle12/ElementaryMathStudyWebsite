﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.ChapterPages
{
    public class IndexPartialModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public IndexPartialModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }
        public IList<Chapter> Chapter { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Chapter = await _context.Chapter
                .Include(c => c.Quiz)
                .Include(c => c.Subject).ToListAsync();


        }
    }
}