using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mecanico_plus.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using mecanico_plus.Pages.Backend.constantes;

namespace mecanico_plus.Pages.Principal.CalendarioMecanico
{
    public class IndexModel : PageModel
    {
   private readonly mecanico_plus.Data.local _context;
       
private readonly DbContextOptions<local> _contextOptions;

       public IndexModel(mecanico_plus.Data.local context, DbContextOptions<local> contextOptions)
       {
           _context = context;
           _contextOptions = contextOptions;
       }


        public IList<t016_auditoria_mecanico> Auditorias { get; set; }
        public IList<t006_mecanico> Mecanicos { get; set; }
        public IList<t002_empresa_o_persona_natural> Empresas { get; set; }

        [BindProperty]
        public t016_auditoria_mecanico t016_auditoria_mecanico { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
             var email = HttpContext.Session.GetString("SessionUser");
    var currentSessionId = HttpContext.Session.GetString("SessionID"); 

    using (var context = new local(_contextOptions))
    {
        var user = await context.t001_usuario
            .FirstOrDefaultAsync(u => u.f001_correo_electronico == email);

        if (user == null || user.f001_sesion_id != currentSessionId)
        {
            TempData["ErrorMessage"] = "La sesión ha caducado o el mismo usuario ingresó en otra sesión en otro navegador. Solo un usuario es permitido por sesión";
            return RedirectToPage("/Login/Index");
        }

               
            }

     try
 {
     if (HttpContext.Session.GetString("SessionUser") != null)
     {

         PermisoDomain permisos = new PermisoDomain();
         if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_CALENDARIO_MECANICOS,
                                                    HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                    Costantes.PERMISO_CONSULTAR))
         {

// Obtén la empresa seleccionada
                        int currentEmpresaId = await ObtenerEmpresaSeleccionada();


        // Load existing auditorias (optional, to display them in the calendar)
            Auditorias = await _context.t016_auditoria_mecanico
                .Include(a => a.vObjMecanico)
                .Include(a => a.vObjEmpresa).Where(t => t.f016_rowid_empresa_o_persona_natural == currentEmpresaId)
                .ToListAsync();

            // Load mechanics and companies
            Mecanicos = await _context.t006_mecanico
                 .Include(t => t.vObjEmpresa)
                 .Where(t => t.f006_rowid_empresa_o_persona_natural == currentEmpresaId)
                 .ToListAsync();
            Empresas = await _context.t002_empresa_o_persona_natural.ToListAsync();

            // Populate ViewData for selects
            ViewData["Mecanicos"] = new SelectList(Mecanicos, "f006_rowid", "f006_nombre", "f006_apellido");
            ViewData["Empresas"] = new SelectList(Empresas, "f002_rowid", "f002_razon_social");
             return null;
         }
         else
         {
             // Mostrar mensaje de error
             TempData["ErrorMessage"] = "No tienes permiso para consultar calendario mecanico.";
             return RedirectToPage("../../Principal/Base/Index");
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
                 public async Task<int> ObtenerEmpresaSeleccionada()
        {
            try
            {
                // Verifica que el correo electrónico del usuario esté disponible en la sesión
                string sessionUser = HttpContext.Session.GetString("SessionUser");
                if (string.IsNullOrEmpty(sessionUser))
                {
                    throw new Exception("Usuario no encontrado en la sesión.");
                }

                // Obtén el rowId de la empresa asociada al usuario logueado
                int rowIdEmpresaSeleccionada = await (from use in _context.t001_usuario
                                                      where use.f001_correo_electronico == sessionUser
                                                      select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

                return rowIdEmpresaSeleccionada;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones (puedes personalizar el mensaje o hacer un registro de errores)
                throw new Exception("Error al obtener la empresa seleccionada.", ex);
            }
        }

        public async Task<IActionResult> OnPostCreateAuditoriaAsync()
        {
            if (t016_auditoria_mecanico.f016_fecha_inicio >= t016_auditoria_mecanico.f016_fecha_finalizacion)
            {
                ModelState.AddModelError(string.Empty, "La fecha/hora de inicio debe ser anterior a la fecha/hora final.");
                
                // Re-populate ViewData in case of error
                Mecanicos = await _context.t006_mecanico.Include(t => t.vObjEmpresa).ToListAsync();
                Empresas = await _context.t002_empresa_o_persona_natural.ToListAsync();
                ViewData["Mecanicos"] = new SelectList(Mecanicos, "f006_rowid", "f006_nombre", "f006_apellido");
                ViewData["Empresas"] = new SelectList(Empresas, "f002_rowid", "f002_razon_social");

                return Page();
            }

            t016_auditoria_mecanico.f016_ts = DateTime.Now;

            _context.t016_auditoria_mecanico.Add(t016_auditoria_mecanico);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
