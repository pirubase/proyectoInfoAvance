using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace mecanico_plus.Pages.Principal.Base
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DbContextOptions<local> _contextOptions;
        private readonly mecanico_plus.Data.local _context;
        public IndexModel(ILogger<IndexModel> logger, mecanico_plus.Data.local context, DbContextOptions<local> contextOptions)
        {
            _logger = logger;
              _context = context;
           _contextOptions = contextOptions;
        }

        public async Task<IActionResult> OnGetAsync()
        {
     

            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    return null;

                }
                else
                {
                    HttpContext.Session.SetString("ExpiredSession", "true");
                    return RedirectToPage("../../Login/Index");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("ExpiredSession", "true");
                return RedirectToPage("../../Login/Index");
            }


        }
    }
}
