using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Perfil
{
    [Authorize]
    public class IndexModel : PageModel
    {
        
         private readonly mecanico_plus.Data.local _context;
       
private readonly DbContextOptions<local> _contextOptions;

       public IndexModel(mecanico_plus.Data.local context, DbContextOptions<local> contextOptions)
       {
           _context = context;
           _contextOptions = contextOptions;
       }


        public IList<t004_perfil> t004_perfil { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {

           
            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_PERFILES,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CONSULTAR))
                    {
                        t004_perfil = await _context.t004_perfil
                   /* .Include(t => t.vObjEmpresa)*/.ToListAsync();

                        return null;
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para consultar perfil.";
                        return RedirectToPage("../../Principal/Base/Index");
                    }

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
        public IActionResult OnPostIrACrear()
        {
            try
            {
                return RedirectToPage("Create");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
