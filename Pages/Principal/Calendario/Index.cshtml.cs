using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.ModeloConDatos;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Pages.Backend.accesoDatos;
using System.Drawing.Text;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Calendario
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


        public IList<t009_cita> Citas { get; set; }
        public IList<t009_cita> CitasPendientes { get; set; }

        [BindProperty]
        public t009_cita t009_cita { get; set; }

        public List<TipoServicio> NombreTipoServicios { get; set; }

        public int? CurrentClientId { get; private set; }

        public IList<t006_mecanico> Mecanicos { get; set; }
        public t006_mecanico MecanicoPreferido { get; set; }
        public t006_mecanico MecanicoFamiliaActual { get; set; }
        public int CurrentPatientId { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
               
    // Obtén la empresa seleccionada
                        int currentEmpresaId = await ObtenerEmpresaSeleccionada();

                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_CALENDARIOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CONSULTAR))
                    {
                        var userType = HttpContext.Session.GetString("UserType");
                        var userEmail = HttpContext.Session.GetString("SessionUser");

                        // Cargar todas las citas primero
                        var todasLasCitas = await _context.t009_cita
                            .Include(c => c.vObjMecanico) // Incluir vObjDoctor
                            .Include(c => c.vObjEmpresa)
                            .Include(c => c.vObjEspecialidad)
                            .Include(c => c.vObjCliente)
                            .Include(c => c.vObjUsuarioAprobador)
                            .Include(c => c.vObjUsuarioCreador)
                            .Include(c => c.vObjServicio)
                            .Where(c => c.f009_estado != "Finalizada").Where(t => t.f009_rowid_empresa_o_persona_natural == currentEmpresaId)
                            .ToListAsync();

                        if (userType == "Cliente")
                        {
                            var user = await _context.t001_usuario
                                .Include(u => u.vObjCliente)
                                .ThenInclude(p => p.vObjMecanicoFamilia)
                                .Where(t => t.f001_rowid_empresa_o_persona_natural == currentEmpresaId)
                                .FirstOrDefaultAsync(u => u.f001_correo_electronico == userEmail);

                            if (user?.vObjCliente != null)
                            {
                                CurrentPatientId = user.vObjCliente.f007_rowid;
                                MecanicoFamiliaActual = user.vObjCliente.vObjMecanicoFamilia;
                                
                                // Asegurarse de incluir la especialidad al cargar los doctores
                                Mecanicos = await _context.t006_mecanico
                                   
                                    .OrderBy(d => d.f006_apellido)
                                    .ToListAsync();

                                // Modificar para incluir todas las citas del paciente (pendientes, rechazadas y aprobadas)
                                CitasPendientes = todasLasCitas
                                    .Where(c => c.f009_rowid_cliente == user.vObjCliente.f007_rowid && 
                                           (c.f009_estado == "Pendiente" || 
                                            c.f009_estado == "Rechazada" || 
                                            c.f009_estado == "Aprobada" ||  c.f009_estado == "en proceso" ||  c.f009_estado == "diagnóstico"  || c.f009_estado == "asignada"))
                                    .ToList();
                            }
                        }

                        // Para el calendario, mostrar solo las citas aprobadas
                        Citas = todasLasCitas.Where(c => c.f009_estado != "finalizada").ToList();

                        // Para usuarios no pacientes, mostrar todas las citas pendientes
                        if (userType != "Cliente")
                        {
                            CitasPendientes = Citas
                                .Where(c => c.f009_estado == "Pendiente" || 
                                            c.f009_estado == "Rechazada" || 
                                            c.f009_estado == "Aprobada" ||  c.f009_estado == "en proceso" ||  c.f009_estado == "diagnóstico"  || c.f009_estado == "asignada")
                                .ToList();
                        }

                        ViewData["ListaServiciosConValor"] = await _context.t014_servicio
                            .Select(s => new {
                                f014_rowid = s.f014_rowid,
                                f014_valor = s.f014_valor
                            })
                            .ToListAsync();

                        ConsultarItemsForaneos();
                        return null;
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para consultar calendarios de citas.";
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

        public async Task<IActionResult> OnPostCreateCitaAsync()
        {
            try
            {
                DateTime f009_hora;
                string f009_observacion = Request.Form["f009_observacion"];
                int doctorId;
                int f009_tipo_cita;
                int pacienteId;
                int especialidadId;
                int empresaId;

                if (!DateTime.TryParse(Request.Form["f009_hora"], out f009_hora) ||
                    !int.TryParse(Request.Form["doctorId"], out doctorId) ||
                    !int.TryParse(Request.Form["f009_tipo_cita"], out f009_tipo_cita) ||
                    !int.TryParse(Request.Form["pacienteId"], out pacienteId) ||
                    !int.TryParse(Request.Form["especialidadId"], out especialidadId) ||
                    !int.TryParse(Request.Form["empresaId"], out empresaId))
                {
                    TempData["ErrorMessage"] = "Datos inválidos o incompletos.";
                    ConsultarItemsForaneos();
                    return Page();
                }

                var userType = HttpContext.Session.GetString("UserType");
                var userEmail = HttpContext.Session.GetString("SessionUser");
                var currentUser = await _context.t001_usuario
                    .FirstOrDefaultAsync(u => u.f001_correo_electronico == userEmail);

                var pacienteInfo = await _context.t007_cliente
                    .FirstOrDefaultAsync(p => p.f007_rowid == pacienteId);

                var doctorInfo = await _context.t006_mecanico
                    .FirstOrDefaultAsync(p => p.f006_rowid == doctorId);

                var especialidadInfo = await _context.t010_vehiculo
                    .FirstOrDefaultAsync(p => p.f010_rowid == especialidadId);

                var servicioInfo = await _context.t014_servicio
                     .FirstOrDefaultAsync(p => p.f014_rowid == f009_tipo_cita);

                if (pacienteInfo == null || doctorInfo == null || especialidadInfo == null)
                {
                    TempData["ErrorMessage"] = "Datos inválidos o incompletos.";
                    ConsultarItemsForaneos();
                    return Page();
                }

                var datosCita = new mecanico_plus.APIs.DatosCitaDTO
                {
                    f009_hora = f009_hora,
                    NombreTipoServicio = servicioInfo.f014_nombre,
                    f009_observacion = f009_observacion ?? string.Empty,
                    PacienteCorreo = pacienteInfo.f007_correo,
                    PacienteNombre = $"{pacienteInfo.f007_nombre} {pacienteInfo.f007_apellido}",
                    DoctorNombre = $"{doctorInfo.f006_nombre} {doctorInfo.f006_apellido}",
                    NombreEspecializacion = especialidadInfo.f010_nombre
                };

                // Preparar cliente HTTP para llamar a la APIgenérica
                var apiGenerica = new mecanico_plus.APIs.APIGenerica();
                var response = await apiGenerica.CrearCitaConCorreo(datosCita);

                if (response is OkObjectResult)
                {
                    var nuevaCita = new t009_cita
                    {
                        f009_ts = DateTime.Now,
                        f009_hora = f009_hora,
                        f009_fecha_inicio = f009_hora, // Ensure this is set
                        f009_fecha_finalizacion = f009_hora.AddHours(1), // Ensure this is set
                        f009_descripcion = f009_observacion ?? string.Empty,
                        f009_rowid_servicio = servicioInfo.f014_rowid,
                        f009_rowid_mecanico = doctorId,
                        f009_rowid_cliente = pacienteId,
                        f009_rowid_especialidad = especialidadId,
                        f009_rowid_empresa_o_persona_natural = empresaId,
                        f009_estado = userType == "Cliente" ? "Pendiente" : "Aprobada",
                        f009_rowid_usuario_creador = currentUser.f001_rowid,
                        f009_rowid_usuario_aprobador = userType != "Cliente" ? currentUser.f001_rowid : (int?)null
                    };

                    _context.t009_cita.Add(nuevaCita);
                    await _context.SaveChangesAsync();

                    // Asegurarse de cargar todas las relaciones necesarias
                    await _context.Entry(nuevaCita)
                        .Reference(c => c.vObjMecanico)
                        .LoadAsync();
                    await _context.Entry(nuevaCita)
                        .Reference(c => c.vObjCliente)
                        .LoadAsync();
                    await _context.Entry(nuevaCita)
                        .Reference(c => c.vObjEspecialidad)
                        .LoadAsync();

                   
                    TempData["SuccessMessage"] = "Cita creada y correo enviado correctamente.";
                    return RedirectToPage();
                }

                TempData["ErrorMessage"] = "Error al crear la cita.";
                ConsultarItemsForaneos();
                return Page();
            }
            catch (Exception ex)
            {
                ConsultarItemsForaneos();
                TempData["ErrorMessage"] = "Error al crear la cita: " + ex.Message;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAprobarCitaAsync(int citaId)
        {
            var userEmail = HttpContext.Session.GetString("SessionUser");
            var currentUser = await _context.t001_usuario
                .Include(u => u.vObjPerfil)
                .FirstOrDefaultAsync(u => u.f001_correo_electronico == userEmail);

            if (currentUser?.vObjPerfil?.f004_nombre != "Doctor")
            {
                TempData["ErrorMessage"] = "Solo los médicos pueden aprobar citas.";
                return RedirectToPage();
            }

            var cita = await _context.t009_cita.FindAsync(citaId);
            if (cita == null)
            {
                return NotFound();
            }

            cita.f009_estado = "Aprobada";
            cita.f009_rowid_usuario_aprobador = currentUser.f001_rowid;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cita aprobada correctamente.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRechazarCitaAsync(int citaId)
        {
            var userEmail = HttpContext.Session.GetString("SessionUser");
            var currentUser = await _context.t001_usuario
                .Include(u => u.vObjPerfil)
                .FirstOrDefaultAsync(u => u.f001_correo_electronico == userEmail);

            if (currentUser?.vObjPerfil?.f004_nombre != "Doctor")
            {
                TempData["ErrorMessage"] = "Solo los médicos pueden rechazar citas.";
                return RedirectToPage();
            }

            var cita = await _context.t009_cita.FindAsync(citaId);
            if (cita == null)
            {
                return NotFound();
            }

            cita.f009_estado = "Rechazada";
            cita.f009_rowid_usuario_aprobador = currentUser.f001_rowid;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cita rechazada correctamente.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCitaDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userType = HttpContext.Session.GetString("UserType");
            if (userType == "Cliente")
            {
                var userEmail = HttpContext.Session.GetString("SessionUser");
                var user = await _context.t001_usuario
                    .Include(u => u.vObjCliente)
                    .FirstOrDefaultAsync(u => u.f001_correo_electronico == userEmail);

                var cita = await _context.t009_cita
                    .Include(c => c.vObjMecanico)
                    .Include(c => c.vObjCliente)
                    .Include(c => c.vObjEspecialidad)
                    .Include(c => c.vObjServicio)
                    .FirstOrDefaultAsync(m => m.f009_rowid == id);

                if (cita == null || user?.vObjCliente == null || cita.f009_rowid_cliente != user.vObjCliente.f007_rowid)
                {
                    return new JsonResult(new { error = "No autorizado" });
                }

                return new JsonResult(cita);
            }

            // Original logic for non-patient users
            var regularCita = await _context.t009_cita
                .Include(c => c.vObjMecanico)
                .Include(c => c.vObjCliente)
                .Include(c => c.vObjEspecialidad)
                .Include(c => c.vObjUsuarioCreador)
                .Include(c => c.vObjUsuarioAprobador)
                .Include(c => c.vObjServicio)
                .FirstOrDefaultAsync(m => m.f009_rowid == id);

            if (regularCita == null)
            {
                return NotFound();
            }

            var now = DateTime.Now;
            var citaTime = regularCita.f009_hora;
            var timeUntilCita = citaTime - now;
            var canJoinMeeting = regularCita.f009_rowid_servicio == 2 && 
                                timeUntilCita <= TimeSpan.FromMinutes(15) && 
                                timeUntilCita > TimeSpan.FromHours(-1);

            string token = null;
            if (canJoinMeeting)
            {
                var tokenInfo = await _context.t012_token
                    .Where(t => t.f012_rowid_cita == regularCita.f009_rowid && !t.IsExpired)
                    .OrderByDescending(t => t.f012_ts)
                    .FirstOrDefaultAsync();

                if (tokenInfo == null)
                {
                    // Si no existe un token, crear uno nuevo
                    tokenInfo = new t012_token
                    {
                        f012_ts = DateTime.Now,
                        f012_token = TokenGenerator.GenerateTokenId(), // Usar TokenGenerator
                        f012_rowid_cita = regularCita.f009_rowid,
                        f012_rowid_empresa_o_persona_natural = regularCita.f009_rowid_empresa_o_persona_natural,
                        f012_expiracion = citaTime.AddHours(1)
                    };

                    _context.t012_token.Add(tokenInfo);
                    await _context.SaveChangesAsync();
                }

                token = tokenInfo?.f012_token;
            }

            // Crear el objeto anónimo con TODAS las propiedades necesarias
            var citaDetails = new
            {
                regularCita.f009_rowid,
                regularCita.f009_rowid_servicio,
                regularCita.f009_hora,
                regularCita.f009_estado,
                NombreTipoServicio = regularCita.vObjServicio?.f014_nombre,
                vObjMecanico = regularCita.vObjMecanico,
                vObjCliente = regularCita.vObjCliente,
                vObjEspecialidad = regularCita.vObjEspecialidad,
                vObjUsuarioCreador = regularCita.vObjUsuarioCreador,
                vObjUsuarioAprobador = regularCita.vObjUsuarioAprobador,
                vObjEmpresa = regularCita.vObjEmpresa,
                canJoinMeeting = canJoinMeeting,
                token = token,
                regularCita.f009_ts,
                regularCita.f009_rowid_mecanico,
                regularCita.f009_rowid_cliente,
                regularCita.f009_rowid_especialidad,
                regularCita.f009_rowid_empresa_o_persona_natural,
                regularCita.f009_rowid_usuario_creador,
                regularCita.f009_rowid_usuario_aprobador
            };

            return new JsonResult(citaDetails);
        }

        public async Task<IActionResult> OnPostUpdateCitaStatusAsync([FromForm] int citaId, [FromForm] string newStatus)
        {
            if (string.IsNullOrEmpty(newStatus))
            {
                return BadRequest("El nuevo estado es requerido.");
            }

            var cita = await _context.t009_cita
            .Include(c => c.vObjMecanico)
            .Include(c => c.vObjCliente)
            .Include(c => c.vObjEspecialidad)
            .Include(c => c.vObjServicio)
            .Include(c => c.vObjUsuarioCreador)
            .Include(c => c.vObjUsuarioAprobador)
            .Include(c => c.vObjEmpresa)
            .FirstOrDefaultAsync(c => c.f009_rowid == citaId);
            if (cita == null)
            {
                return NotFound();
            }

            cita.f009_estado = newStatus;
            if (newStatus.Equals("finalizada", StringComparison.OrdinalIgnoreCase) ||
                newStatus.Equals("diagnóstico", StringComparison.OrdinalIgnoreCase) ||
                newStatus.Equals("en proceso", StringComparison.OrdinalIgnoreCase))
            {
                var cliente = await _context.t007_cliente
                    .FirstOrDefaultAsync(p => p.f007_rowid == cita.f009_rowid_cliente);
                if (cliente != null)
                {
                    var gestionCorreos = new GestionCorreos();
                    string asunto = $"Su cita ha cambiado de estado a {newStatus}";
                    string mensaje = $"Estimado(a) {cliente.f007_nombre}, su cita ha cambiado de estado a {newStatus}.";
                    gestionCorreos.enviarCorreo(cliente.f007_correo, asunto, mensaje, null, null);
                }

                
            }
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }

        private void ConsultarItemsForaneos()
        {
            string sessionUser = HttpContext.Session.GetString("SessionUser");
            if (string.IsNullOrEmpty(sessionUser))
            {
                throw new Exception("Usuario no encontrado en la sesión.");
            }

            int empresaId = (from use in _context.t001_usuario
                            where use.f001_correo_electronico == sessionUser
                            select use.f001_rowid_empresa_o_persona_natural).FirstOrDefault();

            NombreTipoServicios = TipoServicio.RetornaOpciones().ToList();
            ViewData["f009_rowid_especialidad"] = new SelectList(_context.t010_vehiculo, "f010_rowid", "f010_nombre");
            ViewData["f009_rowid_doctor"] = new SelectList(_context.t006_mecanico, "f006_rowid", "f006_apellido");
            ViewData["f009_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_razon_social");
            ViewData["f009_rowid_servicio"] = new SelectList(_context.t014_servicio, "f014_rowid", "f014_nombre");

            // Verificar si el usuario es un paciente
            var userType = HttpContext.Session.GetString("UserType");
            if (userType == "Cliente")
            {
                // Obtener el paciente asociado al usuario actual
                var paciente = _context.t001_usuario
                    .Include(u => u.vObjCliente)
                    .Where(u => u.f001_correo_electronico == sessionUser)
                    .Select(u => u.vObjCliente)
                    .FirstOrDefault();

                if (paciente != null)
                {
                    // Validar Doctor de Familia solo para pacientes
                   
                    // Mostrar solo el paciente actual
                    ViewData["f009_rowid_paciente"] = new SelectList(
                        new[] { paciente }, 
                        "f007_rowid", 
                        "f007_apellido"
                    );
                }
            }
            else
            {
                // Para usuarios no pacientes, mostrar todos los pacientes
                ViewData["f009_rowid_paciente"] = new SelectList(_context.t007_cliente, "f007_rowid", "f007_apellido");
            }
        }

        public async Task<IActionResult> OnPostAsignarMecanicoPreferidoAsync(int mecanicoId)
        {
            var userEmail = HttpContext.Session.GetString("SessionUser");
            var paciente = await _context.t001_usuario
                .Include(u => u.vObjCliente)
                .Where(u => u.f001_correo_electronico == userEmail)
                .Select(u => u.vObjCliente)
                .FirstOrDefaultAsync();

            if (paciente != null)
            {
             
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo asignar el doctor de familia.";
            }

            return RedirectToPage();
        }
    }
}
