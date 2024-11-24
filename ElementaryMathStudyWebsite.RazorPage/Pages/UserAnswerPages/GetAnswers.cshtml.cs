using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.RazorPage.Pages.UserAnswerPages
{
    public class GetAnswersModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public GetAnswersModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [BindProperty]
        public string QuizId { get; set; } = string.Empty;

        public List<UserAnswerWithDetailsDTO> UserAnswers { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the current user ID from the session
            string userId = HttpContext.Session.GetString("user_id") ?? "";
            if (string.IsNullOrEmpty(userId))
            {
                ErrorMessage = "User is not logged in.";
                return Page();
            }

            if (string.IsNullOrEmpty(QuizId))
            {
                ErrorMessage = "Quiz ID is required.";
                return Page();
            }

            try
            {
                var response = await _httpClient.GetAsync($"/api/QuizAnswers/quiz/{QuizId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<BaseResponse<BasePaginatedList<UserAnswerWithDetailsDTO>>>(content);

                    // Explicitly convert IReadOnlyCollection to List
                    UserAnswers = apiResponse?.Data.Items.ToList() ?? new List<UserAnswerWithDetailsDTO>();
                }
                else
                {
                    ErrorMessage = "Failed to fetch user answers. Please check the Quiz ID.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return Page();
        }
    }
}
