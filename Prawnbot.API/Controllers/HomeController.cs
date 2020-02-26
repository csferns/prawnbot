using Microsoft.AspNetCore.Mvc;
using Prawnbot.Core.Interfaces;
using Prawnbot.Infrastructure;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Prawnbot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly IBotService botService;
        public HomeController(IBotService botService)
        {
            this.botService = botService;
        }

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetStatus()
        {
            Response<object> status = await botService.GetStatusAsync();
            return Json(status.Entity);
        }
    }
}
