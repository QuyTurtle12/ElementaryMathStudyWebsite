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

        // POST: api/options/{id}
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
                return Ok(await _optionService.AddOption(dto));
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
                if (await _optionService.DeleteOption(id))
                {
                    return Ok("Delete successfully");

                }
                return BadRequest("Delete unsuccessfully");
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
        }


        //// GET: api/options/raw/{id}
        //[Authorize(Policy = "Admin-Content")]
        //[HttpGet]
        //[Route("raw/{id}")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin & Content Manager",
        //    Description = "View an option (of a question)"
        //    )]
        //public async Task<IActionResult> GetOptionById([Required] string id)
        //{
        //    try
        //    {
        //        return Ok(await _optionService.GetOptionById(id));
        //    }
        //    catch (BaseException.CoreException coreEx)
        //    {
        //        // Handle specific CoreException
        //        return StatusCode(coreEx.StatusCode, new
        //        {
        //            code = coreEx.Code,
        //            message = coreEx.Message,
        //            additionalData = coreEx.AdditionalData
        //        });
        //    }
        //    catch (BaseException.BadRequestException badRequestEx)
        //    {
        //        // Handle specific BadRequestException
        //        return BadRequest(new
        //        {
        //            errorCode = badRequestEx.ErrorDetail.ErrorCode,
        //            errorMessage = badRequestEx.ErrorDetail.ErrorMessage
        //        });
        //    }
        //}

        // GET: api/options
        [HttpGet]
        [Route("question")]
        [SwaggerOperation(
            Summary = "Authorization: N/A",
            Description = "View all options of 1 question"
            )]
        public async Task<IActionResult> GetOptionDtosByQuestion([Required] string questionId,int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                var optionAppService = _optionService as IAppOptionServices;
                return Ok(await optionAppService.GetOptionDtosByQuestion(pageNumber, pageSize, questionId));
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
        }


        // GET: api/options/raw
        //[Authorize(Policy = "Admin-Content")]
        //[HttpGet]
        //[Route("raw")]
        //[SwaggerOperation(
        //    Summary = "Authorization: Admin & Content Manager",
        //    Description = "View all options + properties (of a question)"
        //    )]
        //public async Task<IActionResult> GetOptions(int pageNumber = -1, int pageSize = -1)
        //{
        //    try
        //    {
        //        return Ok(await _optionService.GetOptions(pageNumber, pageSize));
        //    }
        //    catch (BaseException.CoreException coreEx)
        //    {
        //        // Handle specific CoreException
        //        return StatusCode(coreEx.StatusCode, new
        //        {
        //            code = coreEx.Code,
        //            message = coreEx.Message,
        //            additionalData = coreEx.AdditionalData
        //        });
        //    }
        //    catch (BaseException.BadRequestException badRequestEx)
        //    {
        //        // Handle specific BadRequestException
        //        return BadRequest(new
        //        {
        //            errorCode = badRequestEx.ErrorDetail.ErrorCode,
        //            errorMessage = badRequestEx.ErrorDetail.ErrorMessage
        //        });
        //    }
        //}

        // PUT: api/options/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpPut]
        [Route("{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "Edit an option (of a question)"
            )]
        public async Task<IActionResult> UpdateOption([Required]string id, [Required] OptionUpdateDto dto)
        {
            try
            {
                return Ok(await _optionService.UpdateOption(id, dto));
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
        }

    }
}
