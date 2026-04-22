using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class FallbackController: Controller // falls back to angular routes when unknown routes is hit
    {
        public ActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "index.html"), "text/HTML");
        }
    }
}