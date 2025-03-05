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

namespace mecanico_plus.Pages.Principal.Perfil
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
                                                               Costantes.PERMISO_EDITAR))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        t004_perfil = await _context.t004_perfil
                            /*.Include(t => t.vObjEmpresa)*/.FirstOrDefaultAsync(m => m.f004_rowid == id);

                        if (t004_perfil == null)
                        {
                            return NotFound();
                        }
                        ViewData["f004_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural, "f002_rowid", "f002_alcance");
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para editar perfil.";
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
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            _context.Attach(t004_perfil).State = EntityState.Modified;

            try
            {
                t004_perfil.f004_ts = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!t004_perfilExists(t004_perfil.f004_rowid))
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

        private bool t004_perfilExists(int id)
        {
            return _context.t004_perfil.Any(e => e.f004_rowid == id);
        }
    }
}
