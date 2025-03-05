using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Permisos
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public EditModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        [BindProperty]
        public t003_permisos t003_permisos { get; set; }

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
                                                               Costantes.PERMISO_EDITAR))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }
                        // Obtener el ID de la empresa asociada al usuario logueado
                        string sessionUser = HttpContext.Session.GetString("SessionUser");
                        if (string.IsNullOrEmpty(sessionUser))
                        {
                            throw new Exception("Usuario no encontrado en la sesión.");
                        }

                        int empresaId = await (from use in _context.t001_usuario
                                               where use.f001_correo_electronico == sessionUser
                                               select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

                        t003_permisos = await _context.t003_permisos
                            .Include(t => t.vObjEmpresa)
                            .Include(t => t.vObjMenu)
                            .Include(t => t.vObjPerfil).FirstOrDefaultAsync(m => m.f003_rowid == id);

                        if (t003_permisos == null)
                        {
                            return NotFound();
                        }
                        ViewData["f003_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_razon_social");
                        ViewData["f003_rowid_menu"] = new SelectList(_context.t005_menu, "f005_rowid", "f005_nombre");
                        ViewData["f003_rowid_perfil"] = new SelectList(_context.t004_perfil, "f004_rowid", "f004_descripcion");
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para editar permisos.";
                        return RedirectToPage("./Index");
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            _context.Attach(t003_permisos).State = EntityState.Modified;

            try
            {
                t003_permisos.f003_ts = DateTime.Now;
                
                // Set all permissions based on menu permission
                t003_permisos.f003_permiso_consultar = t003_permisos.f003_permiso_uso_menu;
                t003_permisos.f003_permiso_crear = t003_permisos.f003_permiso_uso_menu;
                t003_permisos.f003_permiso_editar = t003_permisos.f003_permiso_uso_menu;
                t003_permisos.f003_permiso_detalle = t003_permisos.f003_permiso_uso_menu;
                t003_permisos.f003_permiso_eliminar = t003_permisos.f003_permiso_uso_menu;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!t003_permisosExists(t003_permisos.f003_rowid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool t003_permisosExists(int id)
        {
            return _context.t003_permisos.Any(e => e.f003_rowid == id);
        }
    }
}
