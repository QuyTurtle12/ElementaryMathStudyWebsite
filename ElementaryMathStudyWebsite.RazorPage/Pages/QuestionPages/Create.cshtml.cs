using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Services.Service;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.QuestionPages
{
    public class CreateModel : PageModel
    {
        private readonly IAppQuestionServices _questionService;
        private readonly IAppQuizServices _quizService;
        private readonly IAppUserServices _userService;

        [BindProperty]
        public QuestionCreateDto Question { get; set; } = new QuestionCreateDto();
        public List<SelectListItem> QuizList { get; set; } = new List<SelectListItem>();
        public CreateModel(IAppQuestionServices questionService, IAppUserServices userService, IAppQuizServices quizService)
        {
            _questionService = questionService;
            _userService = userService;
            _quizService = quizService;
        }

        public async Task OnGetAsync()
        {
            List<QuizMainViewDto> quizzes = await _quizService.GetAllQuizzesAsync();
            QuizList = quizzes.Select(q => new SelectListItem
            {
                Value = q.Id,
                Text = q.QuizName     
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Check if the question already exists or not
                string currentUserId = HttpContext.Session.GetString("user_id")!;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["ErrorMessage"] = "User is not authenticated.";
                    return Page();
                }

                User? currentUser = await _userService.GetUserByIdAsync(currentUserId);
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return Page();
                }

                var response = await _questionService.AddQuestionAsync(new List<QuestionCreateDto> { Question }, currentUser);
                if (response == null)
                {
                    TempData["ErrorMessage"] = "One or more questions already exist.";
                    return Page();
                }

                TempData["SuccessMessage"] = "Question has been added successfully!";
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred.";
                return Page();
            }
        }
    }
}
