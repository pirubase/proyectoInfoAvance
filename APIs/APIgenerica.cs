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

namespace mecanico_plus.APIs
{

    public class IdRequest
    {
        public string Id { get; set; }
    }

    public class CorreoInformacion
    {
        public string emailDestino { get; set; }
        public string asunto { get; set; }
        public string mensaje { get; set; }
        public IFormFile archivo { get; set; } // Para recibir el archivo adjunto
    }
    public class DatosCitaDTO
    {
       
        public DateTime f009_hora { get; set; }
        public string NombreTipoServicio { get; set; }
        public string f009_observacion { get; set; }
        public string PacienteCorreo { get; set; }
        public string PacienteNombre { get; set; }

        public string DoctorNombre { get; set; }

        public string NombreEspecializacion { get; set; }
        public string? EnlaceSala { get; set; }
        public string? Token { get; set; }
    }

    public class DatosCitaFinalizarDTO
    {
       
        public DateTime f009_hora { get; set; }
        public string NombreTipoServicio { get; set; }
        public string f009_observacion { get; set; }
        public string PacienteCorreo { get; set; }
        public string PacienteNombre { get; set; }

        public string DoctorNombre { get; set; }

        public string NombreEspecializacion { get; set; }

       
    }

    public class DatosCitaEnviarDTO
    {

        public DateTime f009_hora { get; set; }
        public string NombreTipoServicio { get; set; }
        public string f009_observacion { get; set; }
        public string PacienteCorreo { get; set; }
        public string PacienteNombre { get; set; }

        public string DoctorNombre { get; set; }

        public string NombreEspecializacion { get; set; }

      
    }


    
    [ApiController]
    [Route("api/APIGenerica")]
    public class APIGenerica : ControllerBase
    {

        private local _context;

        private ConexionDB conexion;

        //public IList<t029_servicios> t029_servicios { get; set; }

        //public IList<t030_productos> t030_productos { get; set; }

        //public IList<t032_impuestos> t032_impuestos { get; set; }

        //public IList<t040_item_mov_generico> t040_item_mov_generico { get; set; }


        public APIGenerica()
        {
            conexion = new ConexionDB();

            //creacion del contex
            var optionsBuilder = new DbContextOptionsBuilder<local>();
            optionsBuilder.UseNpgsql(conexion.ObtieneCadenaConexion());

            _context = new local(optionsBuilder.Options);
        }

        [HttpGet("empresaseleccionada/{pvStrTrama}")]
        public async Task<bool> GetEmpresaSeleccionada(string pvStrTrama)
        {
            string[] vStrParametros = pvStrTrama.Split("~");
            try
            {
                EmpresasDomain empresasDomain = new EmpresasDomain();
                return await empresasDomain.actualizaEmpresaSeleccionada(vStrParametros[0], Convert.ToInt16(vStrParametros[1]));
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost("enviarCorreoRecuperacionClave")]
        public IActionResult enviarCorreoRecuperacionClave([FromBody] correo correo)
        {
            string resultado = string.Empty;
            string claveRecuperada = string.Empty;
            LoginDomain loginDomain = new LoginDomain();

            if (loginDomain.correoUsuarioExistente(correo.destinatario))
            {
                GestionCorreos gestionCorreos = new GestionCorreos();
                LoginDAO loginDAO = new LoginDAO();
                claveRecuperada = loginDAO.obtieneClaveUsuarioPorCorreo(correo.destinatario);
                resultado = gestionCorreos.enviarCorreo(correo.destinatario, correo.asunto, correo.mensaje + claveRecuperada, null, null);

                Console.WriteLine("resultado :", resultado);

                // Retorna respuesta JSON exitosa
                return Ok(new { success = true, message = resultado });

            }
            else
            {
                resultado = "No se encuentra suscrito o la suscripción no está activa. Si no está de acuerdo con esta afirmación contacta a soporte.";

                // Retorna respuesta de error
                return BadRequest(new { success = false, message = resultado });
            }

           
        }


        [HttpPost("crearCitaConCorreo")]
      public async Task<IActionResult> CrearCitaConCorreo(DatosCitaDTO cita)
        {
            try
            {
                //// Guardar la cita en la base de datos
                //cita.f009_ts = DateTime.UtcNow;
                //_context.t009_cita.Add(cita);
                //await _context.SaveChangesAsync();

                // Obtener datos del paciente
                

                // Configurar el correo
                var gestionCorreos = new GestionCorreos();
                string asunto = "Confirmación de cita - Mecanico plus";
                string mensaje = $@"
            Estimado/a {cita.PacienteNombre},
            <br><br>
            Su cita ha sido confirmada con los siguientes datos:
            <ul>
                <li><b>Fecha y hora:</b> {cita.f009_hora}</li>
                <li><b>Servicio:</b> {cita.NombreTipoServicio}</li>
                <li><b>Observaciones:</b> {cita.f009_observacion}</li>
                <li><b>Vehiculo:</b> {cita.NombreEspecializacion}</li>
                <li><b>Nombre del mecanico:</b> {cita.DoctorNombre}</li>
            </ul>
            <br>
            Gracias por confiar en Mecanico plus.
        ";

                // Enviar el correo
                gestionCorreos.enviarCorreo(
                    destinatario: cita.PacienteCorreo,
                    asunto: asunto,
                    body: mensaje,
                    archivoAdjunto: null,
                    nombreArchivo: null
                );

                return Ok(new { success = true, message = "Cita creada y correo enviado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("editarCitaConCorreo")]
        public async Task<IActionResult> editarCitaConCorreo([FromBody] DatosCitaDTO cita)
        {
            try
            {
                //// Guardar la cita en la base de datos
                //cita.f009_ts = DateTime.UtcNow;
                //_context.t009_cita.Add(cita);
                //await _context.SaveChangesAsync();

                // Obtener datos del paciente



                // Configurar el correo
                var gestionCorreos = new GestionCorreos();
                string asunto = "Modificacion de cita - Mecanico plus";
                string mensaje = $@"
            Estimado/a {cita.PacienteNombre},
            <br><br>
            Su cita ha sido modificada con los siguientes datos:
            <ul>
                <li><b>Fecha y hora:</b> {cita.f009_hora}</li>
                <li><b>Servicio:</b> {cita.NombreTipoServicio}</li>
                <li><b>Observaciones:</b> {cita.f009_observacion}</li>
                <li><b>Vehiculo:</b> {cita.NombreEspecializacion}</li>
                <li><b>Nombre del mecanico:</b> {cita.DoctorNombre}</li>
            </ul>
            <br>
            Gracias por confiar en Mecanico plus.
        ";

                // Enviar el correo
                gestionCorreos.enviarCorreo(
                    destinatario: cita.PacienteCorreo,
                    asunto: asunto,
                    body: mensaje,
                    archivoAdjunto: null,
                    nombreArchivo: null
                );

                return Ok(new { success = true, message = "Cita creada y correo enviado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("eliminarCitaConCorreo")]
        public async Task<IActionResult> eliminarCitaConCorreo([FromBody] DatosCitaDTO cita)
        {
            try
            {
                //// Guardar la cita en la base de datos
                //cita.f009_ts = DateTime.UtcNow;
                //_context.t009_cita.Add(cita);
                //await _context.SaveChangesAsync();

                // Obtener datos del paciente


                // Configurar el correo
                var gestionCorreos = new GestionCorreos();
                string asunto = "Cancelacion de cita - Mecanico plus";
                string mensaje = $@"
            Estimado/a {cita.PacienteNombre},
            <br><br>
            Se ha cancelado su cita con los siguientes datos:
            <ul>
                <li><b>Fecha y hora:</b> {cita.f009_hora}</li>
                <li><b>Servicio:</b> {cita.NombreTipoServicio}</li>
                <li><b>Observaciones:</b> {cita.f009_observacion}</li>
                <li><b>Vehiculo:</b> {cita.NombreEspecializacion}</li>
                <li><b>Nombre del mecanico:</b> {cita.DoctorNombre}</li>
            </ul>
            <br>
            Gracias por confiar en Mecanico plus.
        ";

                // Enviar el correo
                gestionCorreos.enviarCorreo(
                    destinatario: cita.PacienteCorreo,
                    asunto: asunto,
                    body: mensaje,
                    archivoAdjunto: null,
                    nombreArchivo: null
                );

                return Ok(new { success = true, message = "Cita creada y correo enviado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("finalizarCitaConCorreo")]
        public async Task<IActionResult> finalizarCitaConCorreo([FromBody] DatosCitaFinalizarDTO cita)
        {
            try
            {
                //// Guardar la cita en la base de datos
                //cita.f009_ts = DateTime.UtcNow;
                //_context.t009_cita.Add(cita);
                //await _context.SaveChangesAsync();

                // Obtener datos del paciente


                // Configurar el correo
                var gestionCorreos = new GestionCorreos();
                string asunto = "finalizacion de cita - Mecanico plus";
                string mensaje = $@"
            Estimado/a {cita.PacienteNombre},
            <br><br>
            Se ha finalizado con exito su cita con los siguientes datos:
            <ul>
                <li><b>Fecha y hora:</b> {cita.f009_hora}</li>
                <li><b>Servicio:</b> {cita.NombreTipoServicio}</li>
                <li><b>Observaciones:</b> {cita.f009_observacion}</li>
                <li><b>Vehiculo:</b> {cita.NombreEspecializacion}</li>
                <li><b>Nombre del mecanico:</b> {cita.DoctorNombre}</li>
              

            </ul>
            <br>
            Gracias por confiar en Mecanico plus.
        ";

                // Enviar el correo
                gestionCorreos.enviarCorreo(
                    destinatario: cita.PacienteCorreo,
                    asunto: asunto,
                    body: mensaje,
                    archivoAdjunto: null,
                    nombreArchivo: null
                );

                return Ok(new { success = true, message = "Cita creada y correo enviado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("enviarHistoriaConCorreo")]
        public async Task<IActionResult> enviarHistoriaConCorreo([FromBody] DatosCitaEnviarDTO cita)
        {
            try
            {
                //// Guardar la cita en la base de datos
                //cita.f009_ts = DateTime.UtcNow;
                //_context.t009_cita.Add(cita);
                //await _context.SaveChangesAsync();

                // Obtener datos del paciente

                

                // Configurar el correo
                var gestionCorreos = new GestionCorreos();
                string asunto = "Historia clinica - Doctor Plus";
                string mensaje = $@"
            Estimado/a {cita.PacienteNombre},
            <br><br>
            Sen envia historia clinica con los siguientes datos:
            <ul>
                <li><b>Fecha y hora:</b> {cita.f009_hora}</li>
                <li><b>Tipo de cita:</b> {cita.NombreTipoServicio}</li>
                <li><b>Observaciones:</b> {cita.f009_observacion}</li>
                <li><b>Especialidad:</b> {cita.NombreEspecializacion}</li>
                <li><b>Nombre del doctor:</b> {cita.DoctorNombre}</li>
               

            </ul>
            <br>
            Gracias por confiar en Doctor Plus.
        ";

                // Enviar el correo
                gestionCorreos.enviarCorreo(
                    destinatario: cita.PacienteCorreo,
                    asunto: asunto,
                    body: mensaje,
                    archivoAdjunto: null,
                    nombreArchivo: null
                );

                return Ok(new { success = true, message = "Cita creada y correo enviado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("enviarNotificacionCitaVirtual")]
        public async Task<IActionResult> EnviarNotificacionCitaVirtual(DatosCitaDTO datos)
        {
            try
            {
                var gestionCorreos = new GestionCorreos();
                var mensaje = $@"
            <h2>Su cita virtual está próxima a comenzar</h2>
            <p><strong>Fecha y hora:</strong> {datos.f009_hora:dd/MM/yyyy HH:mm}</p>
            <p><strong>Doctor:</strong> {datos.DoctorNombre}</p>
            <p><strong>Especialidad:</strong> {datos.NombreEspecializacion}</p>
            <br>
            <h3>Acceso a la sala virtual:</h3>
            <p>Tienes dos opciones para unirte a la cita:</p>
            <ol>
                <li>
                    <p><strong>Opción 1:</strong> Usar el enlace directo</p>
                    <p><a href='{datos.EnlaceSala}' style='background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                        Ingresar a la Sala Virtual
                    </a></p>
                </li>
                <br>
                <li>
                    <p><strong>Opción 2:</strong> Usar el token de acceso</p>
                    <p>Token: <strong>{datos.Token}</strong></p>
                    <p>Puedes ingresar este token en la página de citas virtuales.</p>
                </li>
            </ol>
            <br>
            <p><small>Este acceso estará disponible 15 minutos antes de la hora programada.</small></p>";

                gestionCorreos.enviarCorreo(
                    destinatario: datos.PacienteCorreo,
                    asunto: "Recordatorio de Cita Virtual - Doctor Plus",
                    body: mensaje,
                    archivoAdjunto: null,
                    nombreArchivo: null
                );

                return Ok(new { success = true, message = "Notificación enviada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("ObtenerDetallesCita/{citaId}")]
        public async Task<IActionResult> ObtenerDetallesCita(int citaId)
        {
            try
            {
                var cita = await _context.t009_cita
                    .Include(c => c.vObjMecanico)
                    .Include(c => c.vObjCliente)
                    .Include(c => c.vObjEspecialidad)
                    .FirstOrDefaultAsync(m => m.f009_rowid == citaId);

                if (cita == null)
                    return NotFound();

                var now = DateTime.Now;
                var timeUntilCita = cita.f009_hora - now;
                string token = null;

                if (cita.f009_rowid_servicio == 2) // Si es cita virtual
                {
                    var tokenInfo = await _context.t012_token
                        .Where(t => t.f012_rowid_cita == cita.f009_rowid && t.f012_expiracion > DateTime.Now)
                        .OrderByDescending(t => t.f012_ts)
                        .FirstOrDefaultAsync();

                    if (tokenInfo == null && timeUntilCita <= TimeSpan.FromMinutes(15))
                    {
                        // Crear nuevo token si no existe y estamos dentro del tiempo
                        tokenInfo = new t012_token
                        {
                            f012_ts = DateTime.Now,
                            f012_token = TokenGenerator.GenerateTokenId(), // Usar TokenGenerator
                            f012_rowid_cita = cita.f009_rowid,
                            f012_rowid_empresa_o_persona_natural = cita.f009_rowid_empresa_o_persona_natural,
                            f012_expiracion = cita.f009_hora.AddHours(1)
                        };

                        _context.t012_token.Add(tokenInfo);
                        await _context.SaveChangesAsync();
                    }

                    token = tokenInfo?.f012_token;
                }

                var citaDetails = new
                {
                    cita.f009_rowid,
                    cita.f009_hora,
                    cita.f009_descripcion,
                    cita.f009_rowid_servicio,
                    NombreTipoServicio = cita.vObjServicio.f014_nombre,
                    vObjMecanico = cita.vObjMecanico,
                    vObjCliente = cita.vObjCliente,
                    vObjEspecialidad = cita.vObjEspecialidad,
                    token = token,
                    canJoinMeeting = timeUntilCita <= TimeSpan.FromMinutes(15) && timeUntilCita > TimeSpan.FromHours(-1)
                };

                return Ok(citaDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //funcion que valida si el string es un numero
        public bool IsNumeric(string value)
        {
            return int.TryParse(value, out _);
        }

        //[HttpGet("obtenerServicioPorId/{pvStrIdServicio}")]
        //public async Task<IList<t029_servicios>> PostObtenerServicioPorId(String pvStrIdServicio)
        //{
        //    try
        //    {
        //        int rowId = 0;

        //        if (IsNumeric(pvStrIdServicio))
        //        {
        //            t029_servicios = await _context.t029_servicios
        //                                        .Where(x => x.f029_rowid == Int16.Parse(pvStrIdServicio))
        //                                        .ToListAsync();
        //        }

        //        return t029_servicios;
        //    }
        //    catch (Exception)
        //    {

        //        return null;
        //    }
        //}

        //[HttpGet("obtenerProductoPorId/{rowIdProducto}")]
        //public async Task<IList<t030_productos>> PostobtenerProductoPorRowId(String rowIdProducto)
        //{
        //    try
        //    {
        //        int rowId = 0;

        //        if (IsNumeric(rowIdProducto))
        //        {
        //            rowId = Convert.ToInt32(rowIdProducto);
        //            t030_productos = await _context.t030_productos
        //                                .Include(t => t.vObjAlmacen)
        //                                .Where(x => x.f030_rowid == rowId)
        //                                .ToListAsync();
        //        }

        //        return t030_productos;
        //    }
        //    catch (Exception)
        //    {

        //        return null;
        //    }
        //}


        //[HttpGet("obtenerImpuestoPorId/{pvStrIdImpuesto}")]
        //public async Task<IList<t032_impuestos>> PostObtenerImpuestoPorId(String pvStrIdImpuesto)
        //{
        //    try
        //    {
        //        t032_impuestos = await _context.t032_impuestos
        //                                        .Where(x => x.f032_rowid == Int16.Parse(pvStrIdImpuesto))
        //                                        .ToListAsync();

        //        return t032_impuestos;
        //    }
        //    catch (Exception)
        //    {

        //        return null;
        //    }
        //}


        //[HttpPost("cargarDesgloseCuenta")]
        //public string cargarDesgloseCuenta([FromBody] IdCuenta idCuenta)
        //{
        //    string resultado = string.Empty;



        //    return resultado;
        //}


        [HttpGet("obtieneVlrAppSettingAmbiente")]
        public string obtieneVlrAppSettingAmbiente()
        {
            try
            {
                var settingsManager = new AppSettingsManager();
                return settingsManager.GetAmbiente(Costantes.AMBIENTE);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpGet("obtieneVlrAppSettingDian")]
        public string obtieneVlrAppSettingDian()
        {
            try
            {
                var settingsManager = new AppSettingsManager();
                return settingsManager.GetAmbiente(Costantes.DIAN);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpGet("obtieneVlrAppSettingNomina")]
        public string obtieneVlrAppSettingNomina()
        {
            try
            {
                var settingsManager = new AppSettingsManager();
                return settingsManager.GetAmbiente(Costantes.NOMINA);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpGet("obtenerVersion")]
        public string obtenerVersion()
        {
            try
            {
                var settingsManager = new AppSettingsManager();
                return settingsManager.GetAmbiente(Costantes.VERSION);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //[HttpGet("validaPermisoParaMenuDIAN/{usuario}/{nit}")]
        //public async Task<string> validaPermisoParaMenuDIAN(string usuario, string nit)
        //{
        //    try
        //    {
        //        t001_usuario t001_Usuario = await _context.t001_usuario
        //                            .Where(t => t.f001_correo_electronico.Equals(usuario))
        //                            .FirstOrDefaultAsync();

        //        if (t001_Usuario.f001_id_usuario.Equals("admin") &&
        //            (nit.Equals("9016437836") ||
        //             nit.Equals("9016437836_local") ||
        //             nit.Equals("9016437836_dev")))
        //        {
        //            return "true";
        //        }
        //        else
        //        {
        //            return "false";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}


        [HttpPost("enviarCorreo")]
        public async Task<string> enviarCorreo([FromForm] CorreoInformacion correoInformacion)
        {
            string resultado = string.Empty;

            try
            {
                GestionCorreos gestionCorreos = new GestionCorreos();
                byte[] archivoBytes = null;
                string fileName = null;

                // Verificamos si el archivo existe antes de procesarlo
                if (correoInformacion.archivo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await correoInformacion.archivo.CopyToAsync(memoryStream);
                        archivoBytes = memoryStream.ToArray();
                        fileName = correoInformacion.archivo.FileName;
                    }
                }

                // Llamamos al método para enviar el correo, pasando null para archivoBytes y fileName si no hay adjunto
                resultado = gestionCorreos.enviarCorreo(
                    correoInformacion.emailDestino,
                    correoInformacion.asunto,
                    correoInformacion.mensaje,
                    archivoBytes,
                    fileName
                );
            }
            catch (Exception ex)
            {
                resultado = $"Error al enviar el correo: {ex.Message}";
            }

            return resultado;
        }

        // Actualizar el método EnviarCorreo para manejar async/await
        private async Task EnviarCorreo(string destinatario, string asunto, string mensaje)
        {
            var gestionCorreos = new GestionCorreos();
            gestionCorreos.enviarCorreo(destinatario, asunto, mensaje, null, null);
        }

    }
}

