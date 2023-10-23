using Microsoft.AspNetCore.Mvc;

namespace BesteMusic.WebUI.Controllers
{
    public class PianoController : Controller
    {

        [Route("piyano-çal")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
