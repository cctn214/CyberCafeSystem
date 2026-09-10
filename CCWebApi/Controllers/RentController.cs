using CCDomain.Feature.RentFeature;
using CCDomain.Model.RentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentController : ControllerBase
    {
        private readonly RentService _rentService;
        public RentController(RentService rentService)
        {
            _rentService = rentService;
        }

        [HttpPost("create")]
        public IActionResult CreateRent([FromBody] RentCreateRequestModel request)
        {
            var response = _rentService.CreateRent(request);
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
        public IActionResult GetRentById(RentDetailRequestModel request)
        {
            var response = _rentService.GetRentById(request);
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
        public IActionResult GetAllRents()
        {
            var response = _rentService.GetAllRents();
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
        public IActionResult UpdateRent(RentPatchRequestModel request, int id)
        {
            var response = _rentService.UpdateRent(request, id);
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
        public IActionResult DeleteRent(RentDeleteRequestModel request)
        {
            var response = _rentService.DeleteRent(request);
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
