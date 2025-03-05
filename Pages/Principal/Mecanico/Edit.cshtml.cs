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

namespace mecanico_plus.Pages.Principal.Mecanico
{
    public class EditModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public EditModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        [BindProperty]
        public t006_mecanico t006_mecanico { get; set; } = default!;

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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_MECANICOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_EDITAR))
                    {
                       
                        // Obtener el ID de la empresa asociada al usuario logueado
                        string sessionUser = HttpContext.Session.GetString("SessionUser");
                        if (string.IsNullOrEmpty(sessionUser))
                        {
                            throw new Exception("Usuario no encontrado en la sesión.");
                        }

                        int empresaId = await (from use in _context.t001_usuario
                                               where use.f001_correo_electronico == sessionUser
                                               select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

                        if (id == null)
                        {
                            return NotFound();
                        }

                        var t006_mecanico = await _context.t006_mecanico.FirstOrDefaultAsync(m => m.f006_rowid == id);
                        if (t006_mecanico == null)
                        {
                            return NotFound();
                        }
                        this.t006_mecanico = t006_mecanico;
                        ViewData["f006_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_razon_social");
    
                        return Page();
                       
                     
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para editar mecanicos.";
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

            _context.Attach(t006_mecanico).State = EntityState.Modified;

            try
            {
                t006_mecanico.f006_ts
                    = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!t006_mecanicoExists(t006_mecanico.f006_rowid))
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

        private bool t006_mecanicoExists(int id)
        {
            return _context.t006_mecanico.Any(e => e.f006_rowid == id);
        }
    }
}
