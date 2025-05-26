using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using mecanico_plus.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace mecanico_plus.Pages.Principal.IA
{
    public class IndexModel : PageModel
    {
        private readonly InteligenciaArtificial _iaService;
         private readonly mecanico_plus.Data.local _context;
          private readonly EstadisticasService _estadisticasService; // Nuevo



        [BindProperty]
        public string UserMessage { get; set; }

        public List<ChatMessage> ChatHistory { get; set; } = new List<ChatMessage>();

        public IndexModel(mecanico_plus.Data.local context,InteligenciaArtificial iaService, EstadisticasService estadisticasService)
        {
             _context = context;
            _iaService = iaService;
             _estadisticasService = estadisticasService;
            
        }

        public void OnGet()
        {
            
        }

       public async Task<IActionResult> OnPostAsync()
{
    if (string.IsNullOrEmpty(UserMessage))
        return Page();

   
    int empresaId = await ObtenerEmpresaIdDelUsuario(); 

   
    string contexto = await _estadisticasService.ObtenerContextoEstadisticas(empresaId);

    // Crear prompt combinado
    string prompt = $"""
        {contexto}
        
        Usuario: {UserMessage}
        Asistente: 
        """;

    ChatHistory.Add(new ChatMessage { IsUser = true, Content = UserMessage });

    var response = await _iaService.GenerateResponse(prompt); // Enviar contexto + mensaje
    ChatHistory.Add(new ChatMessage { IsUser = false, Content = response });

    UserMessage = string.Empty;
  

    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
    {
        return new PartialViewResult()
        {
            ViewName = "_ChatPartial",
            ViewData = new ViewDataDictionary<IndexModel>(ViewData, this)
        };
    }
    return Page();
}

private async Task<int> ObtenerEmpresaIdDelUsuario()
{
     string sessionUser = HttpContext.Session.GetString("SessionUser");
    // Implementa según tu lógica de negocio (ej: desde la sesión)
    return await _context.t001_usuario
        .Where(u => u.f001_correo_electronico == sessionUser)
        .Select(u => u.f001_rowid_empresa_o_persona_natural)
        .FirstAsync();
}
        public class ChatMessage
    {
        public bool IsUser { get; set; }
        public string Content { get; set; }
    }
    }

    
}
