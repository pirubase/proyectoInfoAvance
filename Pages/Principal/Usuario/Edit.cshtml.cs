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

namespace mecanico_plus.Pages.Principal.Usuario
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
        public t001_usuario t001_usuario { get; set; }
        public bool IsPatient { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {
                    // Verificar si el usuario es Patient
                    IsPatient = HttpContext.Session.GetString("UserType") == "Patient";

                    if (id == null)
                    {
                        return NotFound();
                    }


                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_USUARIOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_EDITAR))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        t001_usuario = await _context.t001_usuario
                            .Include(t => t.vObjEmpresa)
                            .Include(t => t.vObjEstado)
                            .Include(t => t.vObjPerfil)
                               .Include(t => t.vObjCliente).FirstOrDefaultAsync(m => m.f001_rowid == id);

                        if (t001_usuario == null)
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

                        // Filtrar la lista de empresas
                        ViewData["f001_rowid_empresa_o_persona_natural"] = new SelectList(
                            _context.t002_empresa_o_persona_natural
                                .Where(e => e.f002_rowid == empresaId), // Filtra la empresa específica
                            "f002_rowid",
                            "f002_razon_social"
                        );

                        ViewData["f001_rowid_estado"] = new SelectList(_context.t008_estados_usuario, "f008_rowid", "f008_nombre_estado");
                        ViewData["f001_rowid_perfil"] = new SelectList(_context.t004_perfil, "f004_rowid", "f004_descripcion");
                        ViewData["f001_rowid_paciente"] = new SelectList(_context.t007_cliente, "f007_rowid", "f007_nombre");
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para editar usuarios.";
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
            IsPatient = HttpContext.Session.GetString("UserType") == "Patient";

            if (IsPatient)
            {
                // Obtener el registro original
                var originalUser = await _context.t001_usuario.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.f001_rowid == t001_usuario.f001_rowid);

                // Mantener todos los valores originales excepto la contraseña
                t001_usuario.f001_id_usuario = originalUser.f001_id_usuario;
                t001_usuario.f001_nombres = originalUser.f001_nombres;
                t001_usuario.f001_apellidos = originalUser.f001_apellidos;
                t001_usuario.f001_correo_electronico = originalUser.f001_correo_electronico;
                t001_usuario.f001_numero_celular = originalUser.f001_numero_celular;
                t001_usuario.f001_rowid_empresa_o_persona_natural = originalUser.f001_rowid_empresa_o_persona_natural;
                t001_usuario.f001_rowid_estado = originalUser.f001_rowid_estado;
                t001_usuario.f001_rowid_perfil = originalUser.f001_rowid_perfil;
                t001_usuario.f001_rowid_cliente = originalUser.f001_rowid_cliente;
            }

            _context.Attach(t001_usuario).State = EntityState.Modified;

            try
            {

                t001_usuario.f001_ts = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!t001_usuarioExists(t001_usuario.f001_rowid))
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

        private bool t001_usuarioExists(int id)
        {
            return _context.t001_usuario.Any(e => e.f001_rowid == id);
        }
    }
}
