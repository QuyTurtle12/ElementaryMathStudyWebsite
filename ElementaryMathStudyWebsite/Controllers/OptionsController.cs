using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
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
                var optionAppService = _optionService as IAppOptionServices;
                return Ok(await optionAppService.AddOption(dto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }
        //eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2NUZGN0RGRS02QUIyLTQxMTQtOUM5RC03RDkyMDEyOTNBMDQiLCJleHAiOjE3MjY2NDk3OTN9.47LHD01U68DN_9yJn4pmGdjq_NC6vYRoyhcCurTi1O0

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
                var optionAppService = _optionService as IAppOptionServices;
                if (await optionAppService.DeleteOption(id))
                {
                    return Ok("Delete successfully");

                }
                return BadRequest("Delete unsuccessfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }


        // GET: api/options/raw/{id}
        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [Route("raw/{id}")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "View an option (of a question)"
            )]
        public async Task<IActionResult> GetOptionById([Required] string id)
        {
            try
            {
                return Ok(await _optionService.GetOptionById(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

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
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }


        // GET: api/options/raw
        [Authorize(Policy = "Admin-Content")]
        [HttpGet]
        [Route("raw")]
        [SwaggerOperation(
            Summary = "Authorization: Admin & Content Manager",
            Description = "View all options + properties (of a question)"
            )]
        public async Task<IActionResult> GetOptions(int pageNumber = -1, int pageSize = -1)
        {
            try
            {
                return Ok(await _optionService.GetOptions(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
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
        public async Task<IActionResult> UpdateOption([Required]string id, [Required] OptionUpdateDto dto)
        {
            try
            {
                var optionAppService = _optionService as IAppOptionServices;
                return Ok(await optionAppService.UpdateOption(id, dto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}
