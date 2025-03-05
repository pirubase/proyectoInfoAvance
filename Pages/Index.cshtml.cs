using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace mecanico_plus.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            //return RedirectToPage("Principal/Base/Base");
            return RedirectToPage("Login/Index");
        }
    }
}
