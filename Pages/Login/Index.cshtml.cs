using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.genericos;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.accesoDatos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace mecanico_plus.Pages.Login
{
    public class IndexModel : PageModel
    {

        public IConfiguration Configuration { get; }


        [BindProperty]
        public ModeloLogin pubObjLogin { get; set; }
        public string pubStrMensajeUsuario = string.Empty;
        public string pubStrStorage = string.Empty;
        public string pubStrCadenaConexion;

        public string version = string.Empty;

        [BindProperty]
        public ModeloPagoData FormData { get; set; }

        private readonly local _context;


        public IndexModel(IConfiguration configuration)
        {
            Configuration = configuration;
            var settingsManager = new AppSettingsManager();
            version = settingsManager.GetAmbiente(Costantes.VERSION);
        }

        private async Task<string> AppSettings()
        {

            var settingsManager = new AppSettingsManager();

            // Definir variables para enviar a concatenar
            string server = "";
            string port = "";
            string database = "";
            string user = "";
            string password = "";
            string pooling = "";
            string searchPath = ""; // Parametro cambiante

            //string ambiente = settingsManager.GetAmbiente(Constantes.AMBIENTE);
            string puerto = settingsManager.GetAmbiente(Costantes.PUERTO);
            string clave = settingsManager.GetAmbiente(Costantes.CLAVE);

            server = "localhost";
            port = puerto;
            database = "postgres";
            user = "postgres";
            password = clave;
            pooling = "true";
            searchPath = "mecanico_plus"; // Schema

            // Concatenar una cadena de conexi�n
            settingsManager.ConcatenarCadenaConexion(server, port, database, user, password, pooling, searchPath);
            // Modificar una cadena de conexi�n
            settingsManager.SetConnectionString(Costantes.LABEL_CONECTION_STRING);

            // Obtener una cadena de conexi�n
            return settingsManager.GetConnectionString(Costantes.LABEL_CONECTION_STRING);
        }

        

        //public IndexModel(local context)
        //{
        //    _context = context;
        //}

        public void OnGet()
        {
            if (HttpContext.Session.GetString("ExpiredSession") != null)
                this.pubStrMensajeUsuario = "La sesi�n ha caducado";

            // Limpiar la sesi�n actual
            HttpContext.Session.Clear();

        }

        public void OnPost()
        {

        }
        private string GenerarTokenSeguridad(string llaveTerminal)
        {
            var horaActual = DateTime.Now.ToString("HH:mm");
            var cadena = llaveTerminal + "*" + horaActual;
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(cadena);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        private string GenerateJwtToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60), // Token válido por 1 hora
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
      
        public async Task<IActionResult> OnPostLoginAsync()
        {

            Console.WriteLine("Ingreso OnPostLogin");

            string vStrResultado = "Nombre de usuario, clave o identificacion empresa incorrectos";

            try
            {
                FormData.TokenSeguridad = GenerarTokenSeguridad("4db82d6c4bd34a4d90059ee411e4e446");

                // Administra la cadena de conexi�n
                await AppSettings();

                LoginDomain loginDomain = new LoginDomain();
                EmpresasDomain empresasDomain = new EmpresasDomain();

                if (await loginDomain.validaLogin(pubObjLogin))
                {
                    var settingsManager = new AppSettingsManager();

                    //// Obtiene y guarda la cadena de conexion
                   // await loginDomain.actualizaCadenaConexion(settingsManager.GetConnectionString(Costantes.LABEL_CONECTION_STRING));

                    //await empresasDomain.actualizaEmpresaSeleccionadaPorNit(pubObjLogin.correoElectronico, pubObjLogin.identificacionEmpresa);
                    HttpContext.Session.SetString("SessionUser", pubObjLogin.correoElectronico);
                    pubStrStorage = pubObjLogin.correoElectronico;

                    // Get user info from database to check if it's a patient
                    using (var context = new local(new DbContextOptionsBuilder<local>()
                        .UseNpgsql(settingsManager.GetConnectionString(Costantes.LABEL_CONECTION_STRING))
                        .Options))
                    {
                        var user = await context.t001_usuario
                            .FirstOrDefaultAsync(u => u.f001_correo_electronico == pubObjLogin.correoElectronico);
                        
                        if (user != null)
                        {
                            // Expirar la sesión si ha pasado demasiado tiempo desde la última actividad
                            if (user.f001_ultima_actividad.HasValue && (DateTime.Now - user.f001_ultima_actividad.Value).TotalMinutes > 30)
                            {
                                user.f001_sesion_id = null;
                                user.f001_ultima_actividad = null;
                                await context.SaveChangesAsync();
                            }

                            // Quitar lógica que bloquea login si f001_sesion_id ya existe
                            user.f001_sesion_id = Guid.NewGuid().ToString();
                            user.f001_ultima_actividad = DateTime.Now;
                            await context.SaveChangesAsync();

                            // Guarda también el session id en la sesión
                            HttpContext.Session.SetString("SessionID", user.f001_sesion_id);

                            if (user.f001_rowid_cliente.HasValue)
                            {
                                HttpContext.Session.SetString("UserType", "Cliente");
                            }
                            else
                            {
                                HttpContext.Session.SetString("UserType", "Staff");
                            }
                        }
                    }

                    // Generar y almacenar el token JWT solo una vez
                    if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                    {
                        var token = GenerateJwtToken(pubObjLogin.correoElectronico);
                        // Store token in a cookie
                        HttpContext.Response.Cookies.Append("AuthorizationCookie", token);
                        HttpContext.Session.SetString("JwtToken", token);
                        Console.WriteLine("Generated JWT Token: " + token);
                    }

                    return RedirectToPage("/Principal/Base/Index");
                }
                else
                {
                    pubStrMensajeUsuario = vStrResultado;

                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
         //   // Instancia de FuncionesGenericas para encriptar la contrase�a ingresada
         //   var funcionesGenericas = new FuncionesGenericas();
         //   string encryptedPassword = funcionesGenericas.Encrypt(Password);

         //   var user = await _context.t001_usuario
         //.Include(u => u.vObjEmpresa) // Aseg�rate de incluir la relaci�n en tu modelo
         //.FirstOrDefaultAsync(u =>
         //    u.f001_correo_electronico == Username &&
         //    u.f001_clave == encryptedPassword &&
         //    u.vObjEmpresa.f002_nit == Nit // Verificaci�n adicional del NIT
         //);



         //   if (user != null)
         //   {
         //       // Redirige a la p�gina principal o al �rea protegida
         //       return RedirectToPage("/Principal/Prueba/Index");
         //   }
         //   else
         //   {
         //       // Muestra un mensaje de error si el usuario no existe
         //       ModelState.AddModelError(string.Empty, "Usuario o contrase�a incorrectos.");
         //       return Page();
         //   }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var email = HttpContext.Session.GetString("SessionUser");
            if (!string.IsNullOrEmpty(email))
            {
                var settingsManager = new AppSettingsManager();
                using (var context = new local(new DbContextOptionsBuilder<local>()
                    .UseNpgsql(settingsManager.GetConnectionString(Costantes.LABEL_CONECTION_STRING))
                    .Options))
                {
                    var user = await context.t001_usuario
                        .FirstOrDefaultAsync(u => u.f001_correo_electronico == email);

                    if (user != null)
                    {
                        user.f001_sesion_id = null;
                        user.f001_ultima_actividad = null;
                        await context.SaveChangesAsync();
                    }
                }
            }

            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete("AuthorizationCookie");

            return RedirectToPage("/Login/Index");
        }

        // Funci�n para hashear la contrase�a
private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
