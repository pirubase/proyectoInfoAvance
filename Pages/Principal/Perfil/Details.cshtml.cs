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
    public class DetailsModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public DetailsModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        public t004_perfil t004_perfil { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }


                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_PERFILES,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_DETALLE))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        t004_perfil = await _context.t004_perfil
                           /* .Include(t => t.vObjEmpresa)*/.FirstOrDefaultAsync(m => m.f004_rowid == id);

                        if (t004_perfil == null)
                        {
                            return NotFound();
                        }
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para ver detalles de perfil.";
                        return RedirectToPage("./Index");
                    }


                }
                else
                {
                    HttpContext.Session.SetString("ExpiredSession", "true");
                    return RedirectToPage("../../Login/Login");
                };
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("ExpiredSession", "true");
                return RedirectToPage("../../Login/Login");
            }

           
        }
    }
}
