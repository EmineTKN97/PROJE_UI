using Microsoft.AspNetCore.Mvc;

namespace PROJE_UI.ViewComponents
{
    public class Navbar : ViewComponent
    {
        public async Task<IViewComponentResult>InvokeAsync()
        {
            return View(new Navbar());
        }
    }
}
