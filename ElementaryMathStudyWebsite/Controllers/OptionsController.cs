using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace ElementaryMathStudyWebsite.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptionsController : ControllerBase
    {
        private readonly IAppOptionServices _optionService;

        public OptionsController(IAppOptionServices optionService)
        {
            _optionService = optionService;
        }

        // POST: api/options
        [Authorize(Policy = "Admin-Content")]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Add an option (of a question)"
            )]
        public async Task<IActionResult> AddOption(OptionCreateDto dto)
        {
            var response = await _optionService.AddOption(dto);
            return Ok(BaseResponse<OptionViewDto>.OkResponse(response));
        }


        // DELETE: api/options/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpDelete]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Delete an option (of a question)"
            )]
        public async Task<IActionResult> DeleteOption([Required] string id)
        {
            await _optionService.DeleteOption(id);

            return Ok(BaseResponse<string>.OkResponse("Delete successfully"));
        }


        // GET: api/options
        [HttpGet]
        [Route("question")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "View all options of 1 question"
            )]
        public async Task<IActionResult> GetOptionDtosByQuestion([Required] string questionId, int pageNumber = -1, int pageSize = -1)
        {
            var response = await _optionService.GetOptionDtosByQuestion(pageNumber, pageSize, questionId);

            return Ok(BaseResponse<BasePaginatedList<OptionViewDto>>.OkResponse(response));
        }


        // PUT: api/options/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Edit an option (of a question)"
            )]
        public async Task<IActionResult> UpdateOption([Required] string id, [Required] OptionUpdateDto dto)
        {
            var response = await _optionService.UpdateOption(id, dto);

            return Ok(BaseResponse<OptionViewDto>.OkResponse(response));
        }
    }
}