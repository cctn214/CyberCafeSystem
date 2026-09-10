using CCDomain.Feature.UnitFeature;
using CCDomain.Model.UnitModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly UnitService _unitService;
        public UnitController(UnitService unitService)
        {
            _unitService = unitService;
        }

        [HttpPost("create")]
        public IActionResult CreateUnit([FromBody] UnitCreateRequestModel request)
        {
            var response = _unitService.CreateUnit(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("getbyid")]
        public IActionResult GetUnitById(UnitDetailRequestModel request)
        {
            var response = _unitService.GetUnitById(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }

        [HttpGet("getall")]
        public IActionResult GetAllUnits()
        {
            var response = _unitService.GetAllUnits();
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpPatch("update/{id}")]
        public IActionResult UpdateUnit(UnitPatchRequestModel request, int id)
        {
            var response = _unitService.UpdateUnit(request, id);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpDelete("delete")]
        public IActionResult DeleteUnit(UnitDeleteRequestModel request)
        {
            var response = _unitService.DeleteUnit(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return NotFound(response);
            }
        }
    }
}
