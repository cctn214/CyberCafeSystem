using CCDomain.Feature.UserFeature;
using CCDomain.Model.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] UserCreateRequestModel request)
        {
            var response = _userService.CreateUser(request);
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
        public IActionResult GetUserById(UserDetailRequestModel request)
        {
            var response = _userService.GetUserById(request);
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
        public IActionResult GetAllUsers()
        {
            var response = _userService.GetAllUsers();
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
        public IActionResult UpdateUser(UserPatchRequestModel request, int id)
        {
            var response = _userService.UpdateUser(request, id);
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
        public IActionResult DeleteUser(UserDeleteRequestModel request)
        {
            var response = _userService.DeleteUser(request);
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
