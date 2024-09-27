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
        public async Task<IActionResult> AddOption([Required] OptionCreateDto dto)
        {
            try
            {
                var result = BaseResponse<OptionViewDto>.OkResponse(await _optionService.AddOption(dto));
                return Ok(result);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
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
            try
            {
                var result = await _optionService.DeleteOption(id);

                if (result)
                {
                    var successResponse = BaseResponse<string>.OkResponse("Delete successfully");
                    return Ok(successResponse);

                }
                var failedResponse = BaseResponse<string>.OkResponse("Delete unsuccessfully");

                return Ok(failedResponse);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
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
            try
            {
                var result = BaseResponse<BasePaginatedList<OptionViewDto>>.OkResponse(await _optionService.GetOptionDtosByQuestion(pageNumber, pageSize, questionId));
                return Ok(result);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
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
            try
            {
                var result = BaseResponse<OptionViewDto>.OkResponse(await _optionService.UpdateOption(id, dto));

                return Ok(result);
            }
            catch (BaseException.CoreException coreEx)
            {
                // Handle specific CoreException
                return StatusCode(coreEx.StatusCode, new
                {
                    code = coreEx.Code,
                    message = coreEx.Message,
                    additionalData = coreEx.AdditionalData
                });
            }
            catch (BaseException.BadRequestException badRequestEx)
            {
                // Handle specific BadRequestException
                return BadRequest(new
                {
                    errorCode = badRequestEx.ErrorDetail.ErrorCode,
                    errorMessage = badRequestEx.ErrorDetail.ErrorMessage
                });
            }
            catch (BaseException.NotFoundException notFoundEx)
            {
                // Handle general ArgumentException
                return NotFound(new
                {
                    errorCode = notFoundEx.ErrorDetail.ErrorCode,
                    errorMessage = notFoundEx.ErrorDetail.ErrorMessage
                });
            }
        }

    }
}
