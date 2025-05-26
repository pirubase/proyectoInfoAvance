using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.accesoDatos;
using mecanico_plus.Pages.Backend.Conexion;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.Modelos;
using mecanico_plus.Pages.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.APIs; // Asegúrate de tener esta línea para usar TokenGenerator
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Services
{
    // Services/EstadisticasService.cs
public class EstadisticasService
{
    private readonly local _context;

    public EstadisticasService(local context)
    {
        _context = context;
    }

    public async Task<string> ObtenerContextoEstadisticas(int empresaId)
    {
        var mecanicosHoras = await _context.t016_auditoria_mecanico
            .Include(a => a.vObjMecanico)
            .Where(t => t.f016_rowid_empresa_o_persona_natural == empresaId)
            .GroupBy(a => new { a.vObjMecanico.f006_nombre, a.vObjMecanico.f006_apellido })
            .Select(g => new { 
                Nombre = g.Key.f006_nombre, 
                Apellido = g.Key.f006_apellido, 
                TotalHoras = g.Sum(a => (a.f016_fecha_finalizacion - a.f016_fecha_inicio).TotalHours) 
            })
            .OrderByDescending(m => m.TotalHoras)
            .ToListAsync();

        var facturacionTotal = await _context.t009_cita
            .Where(c => c.f009_estado == "finalizada")
            .SumAsync(c => c.vObjServicio.f014_valor);

        // Formatea el contexto como texto
        return $"""
            [Contexto Estadístico]
            - Mecánico con más horas: {mecanicosHoras.FirstOrDefault()?.Nombre} ({mecanicosHoras.FirstOrDefault()?.TotalHoras:N1}h)
            - Facturación total: ${facturacionTotal:N2}
            - Top 3 mecánicos: {string.Join(", ", mecanicosHoras.Take(3).Select(m => $"{m.Nombre} ({m.TotalHoras:N1}h"))}
            """;
    }
}
    
}
