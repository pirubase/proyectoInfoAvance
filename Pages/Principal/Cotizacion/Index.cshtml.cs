using System;
using System.IO;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace mecanico_plus.Pages.Principal.Cotizacion
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


        [BindProperty]
        public string Servicio { get; set; }
        [BindProperty]
        public decimal Valor { get; set; }
        [BindProperty]
        public string DetallesAdicionales { get; set; }

         public async Task<IActionResult> OnGetAsync()
        {
             var email = HttpContext.Session.GetString("SessionUser");
    var currentSessionId = HttpContext.Session.GetString("SessionID"); 

   

              try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_CLIENTES,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CONSULTAR))
                    {
                        // Obtén la empresa seleccionada
                        int currentEmpresaId = await ObtenerEmpresaSeleccionada();



                       
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

        public IActionResult OnPostGenerateCotizacion()
        {
            try
            {
                var pdfDoc = new Document(PageSize.A4, 36f, 36f, 72f, 36f);
                using (var memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();

                    // Configurar fuentes
                    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                    var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.DARK_GRAY);
                    var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);

                    // Añadir logo
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logoo.jpg");
                    if (System.IO.File.Exists(imagePath))
                    {
                        Image logo = Image.GetInstance(imagePath);
                        // Cambiar el tamaño de 140f a 100f para hacer el logo más pequeño
                        logo.ScaleToFit(100f, 100f);
                        logo.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(logo);
                    }

                    // Añadir título
                    Paragraph title = new Paragraph("COTIZACIÓN", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingBefore = 20f;
                    title.SpacingAfter = 20f;
                    pdfDoc.Add(title);

                    // Añadir fecha
                    Paragraph fecha = new Paragraph($"Fecha: {DateTime.Now.ToString("dd/MM/yyyy")}", normalFont);
                    fecha.Alignment = Element.ALIGN_RIGHT;
                    fecha.SpacingAfter = 20f;
                    pdfDoc.Add(fecha);

                    // Crear tabla de detalles
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1f, 2f });
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 20f;

                    // Añadir filas a la tabla
                    AddTableRow(table, "Servicio:", Servicio, headerFont, normalFont);
                    AddTableRow(table, "Valor:", $"${Valor:N2}", headerFont, normalFont);
                    
                    if (!string.IsNullOrEmpty(DetallesAdicionales))
                    {
                        AddTableRow(table, "Detalles Adicionales:", DetallesAdicionales, headerFont, normalFont);
                    }

                    pdfDoc.Add(table);

                    // Añadir términos y condiciones
                    Paragraph terms = new Paragraph("Términos y Condiciones:", headerFont);
                    terms.SpacingBefore = 20f;
                    terms.SpacingAfter = 10f;
                    pdfDoc.Add(terms);

                    Paragraph termsList = new Paragraph(
                        "• Esta cotización tiene una validez de 30 días\n" +
                        "• Los precios pueden variar sin previo aviso\n" +
                        "• No incluye costos adicionales por materiales extra\n",
                        normalFont
                    );
                    pdfDoc.Add(termsList);

                    // Añadir pie de página
                    Paragraph footer = new Paragraph("Gracias por confiar en nuestros servicios", normalFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    footer.SpacingBefore = 30f;
                    pdfDoc.Add(footer);

                    pdfDoc.Close();

                    var pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", $"Cotizacion_{DateTime.Now:yyyyMMdd}.pdf");
                }
            }
            catch (Exception ex)
            {
                // Manejar el error apropiadamente
                return RedirectToPage("./Index");
            }
        }

        private void AddTableRow(PdfPTable table, string label, string value, Font headerFont, Font normalFont)
        {
            PdfPCell cellLabel = new PdfPCell(new Phrase(label, headerFont));
            cellLabel.BorderWidth = 0.5f;
            cellLabel.BackgroundColor = new BaseColor(240, 240, 240);
            cellLabel.Padding = 8f;

            PdfPCell cellValue = new PdfPCell(new Phrase(value, normalFont));
            cellValue.BorderWidth = 0.5f;
            cellValue.Padding = 8f;

            table.AddCell(cellLabel);
            table.AddCell(cellValue);
        }
    }
}
