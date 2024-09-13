using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizsController : ControllerBase
    {
        private readonly IAppQuizServices _quizService;

        public QuizsController(IAppQuizServices QuizService)
        {
            _quizService = QuizService ?? throw new ArgumentNullException(nameof(QuizService));
        }

        [HttpGet("quizzes")]
        public async Task<ActionResult<IList<Quiz>>> GetQuizzes()
        {
            try
            {
                // Lấy danh sách các Quiz từ dịch vụ
                IList<Quiz> quizzes = await _quizService.GetQuizzesAsync();
                return Ok(quizzes);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }

        [HttpGet("quizzes/dtos")]
        public async Task<ActionResult<IList<QuizViewDto>>> GetQuizViewDtos()
        {
            try
            {
                // Lấy danh sách các QuizViewDto từ dịch vụ
                IList<QuizViewDto> quizViewDtos = await _quizService.GetQuizViewDtosAsync();
                return Ok(quizViewDtos);
            }
            catch (Exception ex)
            {
                // Trả về lỗi nếu có vấn đề xảy ra
                return StatusCode(500, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}
