using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mecanico_plus.Pages.Principal.Cliente
{
    public class EstadoReparacionModel : PageModel
    {
        private readonly local _context;

        public EstadoReparacionModel(local context)
        {
            _context = context;
        }

        public List<ReparacionViewModel> Reparaciones { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedEstado { get; set; }
        public List<SelectListItem> EstadosDisponibles { get; set; }

        public async Task<IActionResult> OnGetAsync(int clienteId)
        {
            string sessionUser = HttpContext.Session.GetString("SessionUser");
            var user = await _context.t001_usuario
                .FirstOrDefaultAsync(u => u.f001_correo_electronico == sessionUser);

            var query = _context.t009_cita.AsQueryable();

            if (user != null)
            {
                if (user.f001_rowid_cliente.HasValue)
                {
                    query = query.Where(c =>
                        c.f009_rowid_cliente == user.f001_rowid_cliente.Value &&
                        c.f009_rowid_empresa_o_persona_natural == user.f001_rowid_empresa_o_persona_natural);
                }
                else
                {
                    query = query.Where(c =>
                        c.f009_rowid_empresa_o_persona_natural == user.f001_rowid_empresa_o_persona_natural);
                }
            }

            EstadosDisponibles = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos" },
                new SelectListItem { Value = "en proceso", Text = "En proceso" },
                new SelectListItem { Value = "finalizada", Text = "Finalizada" },
                new SelectListItem { Value = "diagnóstico", Text = "Diagnóstico" },
            };

            if (!string.IsNullOrEmpty(SelectedEstado))
            {
                query = query.Where(c => c.f009_estado == SelectedEstado);
            }

            Reparaciones = await query
                .OrderByDescending(c => c.f009_ts)
                .Select(c => new ReparacionViewModel
                {
                    Estado = c.f009_estado,
                    FechaInicio = c.f009_fecha_inicio,
                    FechaEstimadaFinalizacion = c.f009_fecha_finalizacion,
                    Descripcion = c.f009_descripcion,
                    Servicio = c.vObjServicio.f014_nombre,
                    Mecanico = $"{c.vObjMecanico.f006_nombre} {c.vObjMecanico.f006_apellido}"
                })
                .ToListAsync();

            if (Reparaciones == null || !Reparaciones.Any())
            {
                TempData["ErrorMessage"] = "No se encontró información sobre la reparación de su vehículo.";
            }

            return Page();
        }

        public class ReparacionViewModel
        {
            public string Estado { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime? FechaEstimadaFinalizacion { get; set; }
            public string Descripcion { get; set; }
            public string Servicio { get; set; }
            public string Mecanico { get; set; }
        }
    }
}
