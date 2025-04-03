using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace mecanico_plus.Pages.Principal.Estadistica
{
    [Authorize]
    public class IndexModel : PageModel
    {
 private readonly mecanico_plus.Data.local _context;
       
private readonly DbContextOptions<local> _contextOptions;

       public IndexModel(mecanico_plus.Data.local context, DbContextOptions<local> contextOptions)
       {
           _context = context;
           _contextOptions = contextOptions;
       }


        public class MecanicoHorasViewModel
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public double TotalHoras { get; set; }
        }

        public class ProductividadViewModel
        {
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public int ServiciosCompletados { get; set; }
            public double TiempoPromedioReparacion { get; set; }
        }

        public class FacturacionViewModel
        {
            public decimal TotalFacturado { get; set; }
        }

        public MecanicoHorasViewModel MecanicoConMasHoras { get; set; }
        public List<MecanicoHorasViewModel> MecanicosOrdenadosPorHoras { get; set; }
        public List<ProductividadViewModel> ReporteProductividad { get; set; }
        public FacturacionViewModel ReporteFacturacion { get; set; }

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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_MECANICOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CONSULTAR))
                    {
                        // Obtén la empresa seleccionada
                        int currentEmpresaId = await ObtenerEmpresaSeleccionada();



                        var mecanicosHoras = await _context.t016_auditoria_mecanico
                    .Include(a => a.vObjMecanico)
                    .Where(t => t.f016_rowid_empresa_o_persona_natural == currentEmpresaId)
                    .GroupBy(a => new { a.vObjMecanico.f006_nombre, a.vObjMecanico.f006_apellido })
                    .Select(g => new MecanicoHorasViewModel
                    {
                        Nombre = g.Key.f006_nombre,
                        Apellido = g.Key.f006_apellido,
                        TotalHoras = g.Sum(a => (a.f016_fecha_finalizacion - a.f016_fecha_inicio).TotalHours)
                    })
                    .OrderByDescending(m => m.TotalHoras)
                    .ToListAsync();

                MecanicoConMasHoras = mecanicosHoras.FirstOrDefault();
                MecanicosOrdenadosPorHoras = mecanicosHoras;

                // Reporte de Productividad
                ReporteProductividad = await _context.t009_cita
                    .Where(c => c.f009_estado == "finalizada")
                    .GroupBy(c => new { c.vObjMecanico.f006_nombre, c.vObjMecanico.f006_apellido })
                    .Select(g => new ProductividadViewModel
                    {
                        Nombre = g.Key.f006_nombre,
                        Apellido = g.Key.f006_apellido,
                        ServiciosCompletados = g.Count(),
                        TiempoPromedioReparacion = g.Average(c => (c.f009_fecha_finalizacion - c.f009_fecha_inicio).TotalHours)
                    })
                    .ToListAsync();

                // Reporte de Facturación
                ReporteFacturacion = new FacturacionViewModel
                {
                    TotalFacturado = await _context.t009_cita
                        .Where(c => c.f009_estado == "finalizada")
                        .SumAsync(c => c.vObjServicio.f014_valor)
                };

                

             
                        return null;
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para consultar clientes.";
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
    }
}
