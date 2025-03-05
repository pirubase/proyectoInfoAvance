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

namespace mecanico_plus.Pages.Principal.Usuario
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public DeleteModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        [BindProperty]
        public t001_usuario t001_usuario { get; set; }

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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_USUARIOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_ELIMINAR))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        t001_usuario = await _context.t001_usuario
                            .Include(t => t.vObjEmpresa)
                            .Include(t => t.vObjEstado)
                            .Include(t => t.vObjPerfil)
                             .Include(t => t.vObjCliente)
                            .FirstOrDefaultAsync(m => m.f001_rowid == id);

                        if (t001_usuario == null)
                        {
                            return NotFound();
                        }
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para eliminar doctores.";
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            t001_usuario = await _context.t001_usuario.FindAsync(id);

            if (t001_usuario != null)
            {
                _context.t001_usuario.Remove(t001_usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
