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
using Microsoft.EntityFrameworkCore;

namespace mecanico_plus.Pages.Principal.Servicio
{
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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_SERVICIOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CREAR))
                    {

                        // Obtener el ID de la empresa asociada al usuario logueado
                        string sessionUser = HttpContext.Session.GetString("SessionUser");
                        if (string.IsNullOrEmpty(sessionUser))
                        {
                            throw new Exception("Usuario no encontrado en la sesión.");
                        }

                        int empresaId = await(from use in _context.t001_usuario
                                              where use.f001_correo_electronico == sessionUser
                                              select use.f001_rowid_empresa_o_persona_natural).FirstAsync();
                        ViewData["f014_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_alcance");
                        return Page();

                      

                    }
                    else
                    {

                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para crear mecanicos.";
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
                HttpContext.Session.SetString("ExpiredSession", "true");
                return RedirectToPage("../../Login/Index");
            }
            
        }

        [BindProperty]
        public t014_servicio t014_servicio { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            t014_servicio.f014_ts = DateTime.Now;
            _context.t014_servicio.Add(t014_servicio);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
