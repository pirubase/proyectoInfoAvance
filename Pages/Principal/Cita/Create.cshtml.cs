using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.ModeloConDatos;
using mecanico_plus.Pages.Backend.logicaNegocio;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Pages.Backend.accesoDatos;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Cita
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public CreateModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }
        public IList<TipoServicio> nombreTipoCitas { get; set; }
        


        public async Task<IActionResult> OnGet()
        {
            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_CITAS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CREAR))
                    {
                        ConsultarItemsForaneos();

                        return Page();
                       
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "No tienes permiso para crear citas.";
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

        private void ConsultarItemsForaneos()
        {
            // Obtener el ID de la empresa asociada al usuario logueado
            string sessionUser = HttpContext.Session.GetString("SessionUser");
            if (string.IsNullOrEmpty(sessionUser))
            {
                throw new Exception("Usuario no encontrado en la sesión.");
            }

     

            int empresaId =
                       (from use in _context.t001_usuario
                        where use.f001_correo_electronico == sessionUser
                        select use.f001_rowid_empresa_o_persona_natural).FirstOrDefault();


            nombreTipoCitas = TipoServicio.RetornaOpciones();
            

            ViewData["f009_rowid_especialidad"] = new SelectList(_context.t010_vehiculo, "f010_rowid", "f010_nombre");
            ViewData["f009_rowid_mecanico"] = new SelectList(_context.t006_mecanico, "f006_rowid", "f006_apellido");
            ViewData["f009_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_razon_social");
            ViewData["f009_rowid_cliente"] = new SelectList(_context.t007_cliente, "f007_rowid", "f007_apellido");
            ViewData["f009_rowid_servicio"] = new SelectList(_context.t014_servicio, "f014_rowid", "f014_nombre");
        }

        [BindProperty]
        public t009_cita t009_cita { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            

            try
            {
                // Remove the conversion to UTC
                // t009_cita.f009_hora = DateTime.SpecifyKind(t009_cita.f009_hora, DateTimeKind.Local).ToUniversalTime();


                var paciente = _context.t007_cliente
     .FirstOrDefault(p => p.f007_rowid == t009_cita.f009_rowid_cliente);

                var doctor = _context.t006_mecanico
     .FirstOrDefault(p => p.f006_rowid == t009_cita.f009_rowid_mecanico);

                var especialidad = _context.t010_vehiculo
.FirstOrDefault(p => p.f010_rowid == t009_cita.f009_rowid_especialidad);

                // var datosCita = new
                // {
                //     f009_rowid = t009_cita.f009_rowid,
                //     f009_hora = t009_cita.f009_hora,
                //     NombreTipoCita = t009_cita.NombreTipoCita,
                //     f009_observacion = t009_cita.f009_observacion,
                //     PacienteCorreo = paciente.f007_correo,
                //     PacienteNombre = paciente.f007_nombre +" "+ paciente.f007_apellido,
                //     DoctorNombre = doctor.f006_nombre +" "+ doctor.f006_apellido,
                //     NombreEspecializacion =  especialidad.f010_nombre
                // };
                // Preparar cliente HTTP para llamar a la APIgenérica
                
                var datosCita = new mecanico_plus.APIs.DatosCitaDTO
                {
                    f009_hora = t009_cita.f009_hora,
                    NombreTipoServicio = t009_cita.vObjServicio.f014_nombre,
                    f009_observacion =" t009_cita.f009_observacion",
                    PacienteCorreo = paciente.f007_correo,
                    PacienteNombre = paciente.f007_nombre + " " + paciente.f007_apellido,
                    DoctorNombre = doctor.f006_nombre + " " + doctor.f006_apellido,
                    NombreEspecializacion = especialidad.f010_nombre
                };
                var apiGenerica = new mecanico_plus.APIs.APIGenerica();
                var response = await apiGenerica.CrearCitaConCorreo(datosCita);

                if (response is OkObjectResult)
                {

                    // Guardar la cita en la base de datos
                    t009_cita.f009_ts = DateTime.Now;   //DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
                    t009_cita.f009_hora = DateTime.Now;
                    _context.t009_cita.Add(t009_cita);
                    await _context.SaveChangesAsync();



                 
              

                    return RedirectToPage("Index");
                }
                else
                {


                    ConsultarItemsForaneos();

                    TempData["SuccessMessage"] = "Error al crear la cita, intenta nuevamente.";

                    return Page();
                }
            }
            catch (Exception ex)
            {
                ConsultarItemsForaneos();
                TempData["SuccessMessage"] = "Error al crear la cita, intenta nuevamente.";
                return Page();
            }

        }
    }
}
