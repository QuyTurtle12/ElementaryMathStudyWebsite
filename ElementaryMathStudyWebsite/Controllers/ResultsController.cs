using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using ElementaryMathStudyWebsite.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly IAppResultService _resultService;

        // Constructor
        public ResultsController(IAppResultService resultService)
        {
            _resultService = resultService;
        }

        // POST: api/results
        // Add Result
        [Authorize(Policy = "Student")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Student",
            Description = "Save student quiz's result"
            )]
        public async Task<ActionResult<BaseResponse<BasePaginatedList<Result>>>>AddResult(ResultCreateDto result)
        {

        }
    }
}
