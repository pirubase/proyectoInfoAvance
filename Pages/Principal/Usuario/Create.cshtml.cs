using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Usuario
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
            // Obtener el ID de la empresa asociada al usuario logueado
            string sessionUser = HttpContext.Session.GetString("SessionUser");
            if (string.IsNullOrEmpty(sessionUser))
            {
                throw new Exception("Usuario no encontrado en la sesión.");
            }

            int empresaId = await(from use in _context.t001_usuario
                                  where use.f001_correo_electronico == sessionUser
                                  select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

            // Filtrar la lista de empresas
            ViewData["f001_rowid_empresa_o_persona_natural"] = new SelectList(
                _context.t002_empresa_o_persona_natural
                    .Where(e => e.f002_rowid == empresaId), // Filtra la empresa específica
                "f002_rowid",
                "f002_razon_social"
            );

           
            ViewData["f001_rowid_estado"] = new SelectList(_context.t008_estados_usuario, "f008_rowid", "f008_nombre_estado");
            ViewData["f001_rowid_perfil"] = new SelectList(_context.t004_perfil, "f004_rowid", "f004_descripcion");
            ViewData["f001_rowid_cliente"] = new SelectList(
                _context.t007_cliente.Select(p => new
                {
                    f007_rowid = p.f007_rowid,
                    DisplayText = $"{p.f007_nombre} {p.f007_apellido}"
                }),
                "f007_rowid",
                "DisplayText"
            ).Prepend(new SelectListItem { Text = "-- Seleccione --", Value = "" });
            
            return Page();
        }

        [BindProperty]
        public t001_usuario t001_usuario { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            t001_usuario.f001_ts = DateTime.Now;
            _context.t001_usuario.Add(t001_usuario);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
