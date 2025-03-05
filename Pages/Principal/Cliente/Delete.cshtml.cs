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

namespace mecanico_plus.Pages.Principal.Cliente
{
    public class DeleteModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public DeleteModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        [BindProperty]
        public t007_cliente t007_cliente { get; set; } = default!;

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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_PERMISOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_ELIMINAR))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        var t007_cliente = await _context.t007_cliente.FirstOrDefaultAsync(m => m.f007_rowid == id);

                        if (t007_cliente == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            this.t007_cliente = t007_cliente;
                        }
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para eliminar permisos.";
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

            var t007_cliente = await _context.t007_cliente.FindAsync(id);
            if (t007_cliente != null)
            {
                t007_cliente = t007_cliente;
                _context.t007_cliente.Remove(t007_cliente);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
