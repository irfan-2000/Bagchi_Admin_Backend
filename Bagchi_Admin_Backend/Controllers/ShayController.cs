using Bagchi_Admin_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bagchi_Admin_Backend.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ShayController : Controller
    {

        private readonly IShayariService _shayService;

        public ShayController(IShayariService shayService)
        {
            _shayService = shayService;
        }

         [HttpGet("GetAllShayaris")]
        public async Task<ActionResult<List<ShayariDto>>> GetAllShayaris()
        {
            var shayaris = await _shayService.GetAllShayaris();
            return Ok(shayaris);
        }

        // GET: api/Shayari/5
        [HttpGet("GetById")]
        public async Task<ActionResult<ShayariDto>> GetById(int id)
        {
            var shayari = await _shayService.GetShayariById(id);
            if (shayari == null) return NotFound();
            return Ok(shayari);
        }

        // GET: api/Shayari/category/3
        [HttpGet("GetShayarisByCategory")]
        public async Task<ActionResult<List<ShayariDto>>> GetShayarisByCategory(int categoryId)
        {
            var shayaris = await _shayService.GetShayarisByCategory(categoryId);
            return Ok(shayaris);
        }


        [HttpPost("CreateUser")]
        public async Task<ActionResult<int>> CreateUser([FromBody] UserDto user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
                return BadRequest("Invalid user data");

            // Capture IP Address
            var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            user.IPAddress = remoteIp;

            int newUserId = await _shayService.InsertUser(user);

            if (newUserId > 0)
                return Ok(newUserId); // Returns the newly created UserId

            return StatusCode(500, "Error inserting user");
        }



    }
}
