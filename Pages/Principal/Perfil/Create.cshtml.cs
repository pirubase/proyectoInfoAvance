using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Perfil
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public CreateModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_PERFILES,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CREAR))
                    {

                        ViewData["f004_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural, "f002_rowid", "f002_alcance");
                        return Page();
                    }
                    else
                    {

                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para crear perfil.";
                        return RedirectToPage("./Index");
                    }

                }
                else
                {
                    HttpContext.Session.SetString("ExpiredSession", "true");
                    return RedirectToPage("../../Login/Index");
                };
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No tienes permiso para crear permisos.";
                return RedirectToPage("../../Login/Index");
            }
            
        }

        [BindProperty]
        public t004_perfil t004_perfil { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            t004_perfil.f004_ts = DateTime.Now;
            _context.t004_perfil.Add(t004_perfil);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
