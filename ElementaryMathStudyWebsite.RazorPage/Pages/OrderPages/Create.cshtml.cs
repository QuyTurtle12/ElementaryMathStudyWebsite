﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.OrderPages
{
    public class CreateModel : PageModel
    {
        private readonly ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext _context;

        public CreateModel(ElementaryMathStudyWebsite.Infrastructure.Context.DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CustomerId"] = new SelectList(_context.User, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Order.Add(Order);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
