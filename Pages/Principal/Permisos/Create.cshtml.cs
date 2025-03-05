using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Data;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using mecanico_plus.Pages.Backend.constantes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Permisos
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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_PERMISOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CREAR))
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

                        ViewData["f003_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_razon_social");
                        ViewData["f003_rowid_menu"] = new SelectList(_context.t005_menu, "f005_rowid", "f005_nombre");
                        ViewData["f003_rowid_perfil"] = new SelectList(_context.t004_perfil, "f004_rowid", "f004_descripcion");
                        return Page();
                    }
                    else
                    {

                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para crear permisos.";
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
        public t003_permisos t003_permisos { get; set; }

        [BindProperty]
public string AccionPermiso { get; set; } // Nueva propiedad para la acción de permisos


        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            t003_permisos.f003_ts = DateTime.Now;
            
           // Asignar o quitar permisos según la acción seleccionada
          if (AccionPermiso == "Asignar")
          {
          t003_permisos.f003_permiso_consultar = true;
          t003_permisos.f003_permiso_crear = true;
          t003_permisos.f003_permiso_editar = true;
          t003_permisos.f003_permiso_detalle = true;
          t003_permisos.f003_permiso_eliminar = true;
           t003_permisos.f003_permiso_uso_menu = true;
           }
           else if (AccionPermiso == "Quitar")
           {
          t003_permisos.f003_permiso_consultar = false;
          t003_permisos.f003_permiso_crear = false;
          t003_permisos.f003_permiso_editar = false;
          t003_permisos.f003_permiso_detalle = false;
          t003_permisos.f003_permiso_eliminar = false;
          t003_permisos.f003_permiso_uso_menu = false;
    }
            _context.t003_permisos.Add(t003_permisos);
            await _context.SaveChangesAsync();


            // Volver a cargar la lista de opciones en ViewData
            var sessionUser = HttpContext.Session.GetString("SessionUser");
            int empresaId = await (from use in _context.t001_usuario
                                   where use.f001_correo_electronico == sessionUser
                                   select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

            ViewData["f003_rowid_empresa_o_persona_natural"] = new SelectList(
                _context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId),
                "f002_rowid", "f002_razon_social");

            ViewData["f003_rowid_menu"] = new SelectList(_context.t005_menu, "f005_rowid", "f005_nombre");
            ViewData["f003_rowid_perfil"] = new SelectList(_context.t004_perfil, "f004_rowid", "f004_descripcion");

            return Page();

        }
        public async Task<JsonResult> OnGetPermisosActualesAsync(int menuId, int perfilId)
{
    var permisos = await _context.t003_permisos
        .FirstOrDefaultAsync(p => p.f003_rowid_menu == menuId && p.f003_rowid_perfil == perfilId);

    if (permisos == null)
    {
        return new JsonResult(new
        {
            consultar = false,
            crear = false,
            editar = false,
            detalle = false,
            eliminar = false,
            usoMenu = false
        });
    }

    return new JsonResult(new
    {
        consultar = permisos.f003_permiso_consultar,
        crear = permisos.f003_permiso_crear,
        editar = permisos.f003_permiso_editar,
        detalle = permisos.f003_permiso_detalle,
        eliminar = permisos.f003_permiso_eliminar,
        usoMenu = permisos.f003_permiso_uso_menu
    });
}

public async Task<JsonResult> OnGetAllPermisosAsync(int perfilId)
{
    var menus = await _context.t005_menu.ToListAsync();
    var resultado = new List<object>();

    foreach (var menu in menus)
    {
        var permiso = await _context.t003_permisos
            .FirstOrDefaultAsync(p => p.f003_rowid_perfil == perfilId && p.f003_rowid_menu == menu.f005_rowid);

        bool tienePermiso = permiso != null && (
            permiso.f003_permiso_consultar ||
            permiso.f003_permiso_crear ||
            permiso.f003_permiso_editar ||
            permiso.f003_permiso_detalle ||
            permiso.f003_permiso_eliminar ||
            permiso.f003_permiso_uso_menu
        );

        resultado.Add(new
        {
            Menu = menu.f005_nombre,
            HasPerm = tienePermiso
        });
    }

    return new JsonResult(resultado);
}
    }
}
