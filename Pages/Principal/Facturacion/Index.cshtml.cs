using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using mecanico_plus.Pages.Backend.constantes;

namespace mecanico_plus.Pages.Principal.Facturacion
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

        public IList<t009_cita> Citas { get; set; }
        public t009_cita SelectedCita { get; set; }
        public decimal TotalCost { get; set; }
        public string AdditionalDetails { get; set; }
        public decimal AdditionalCost { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime IssuanceDate { get; set; }

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

                        Citas = await _context.t009_cita
                            .Include(c => c.vObjMecanico)
                            .Include(c => c.vObjCliente)
                            .Include(c => c.vObjServicio)
                            .Where(t => t.f009_rowid_empresa_o_persona_natural == currentEmpresaId).ToListAsync();
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

        public async Task<IActionResult> OnPostGenerateInvoiceAsync(int citaId, decimal additionalCost, string additionalDetails, string paymentMethod)
        {
            if (citaId == 0)
            {
                TempData["ErrorMessage"] = "Cita no seleccionada.";
                return RedirectToPage();
            }

            SelectedCita = await _context.t009_cita
                .Include(c => c.vObjMecanico)
                .Include(c => c.vObjCliente)
                .Include(c => c.vObjServicio)
                .Include(c => c.vObjEmpresa)
                .Include(c => c.vObjEspecialidad)
                .FirstOrDefaultAsync(c => c.f009_rowid == citaId);

            if (SelectedCita == null)
            {
                TempData["ErrorMessage"] = "Cita no encontrada.";
                return RedirectToPage();
            }

            TotalCost = (SelectedCita.vObjServicio?.f014_valor ?? 0) + additionalCost;
            AdditionalDetails = additionalDetails;
            AdditionalCost = additionalCost;
            PaymentMethod = paymentMethod;
            IssuanceDate = DateTime.Now;

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
                        logo.ScaleToFit(100f, 100f);
                        logo.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(logo);
                    }

                    // Añadir título
                    Paragraph title = new Paragraph("FACTURA", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    title.SpacingBefore = 20f;
                    title.SpacingAfter = 20f;
                    pdfDoc.Add(title);

                    // Añadir información de fecha
                    Paragraph fecha = new Paragraph($"Fecha de Emisión: {IssuanceDate:dd/MM/yyyy}", normalFont);
                    fecha.Alignment = Element.ALIGN_RIGHT;
                    fecha.SpacingAfter = 20f;
                    pdfDoc.Add(fecha);

                    // Crear tabla de detalles
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1f, 2f });
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 20f;

                    // Añadir información del cliente y servicio
                    AddTableRow(table, "Cliente:", $"{SelectedCita.vObjCliente.f007_nombre} {SelectedCita.vObjCliente.f007_apellido}", headerFont, normalFont);
                    AddTableRow(table, "Servicio:", SelectedCita.vObjServicio.f014_nombre, headerFont, normalFont);
                    AddTableRow(table, "Fecha de Cita:", SelectedCita.f009_hora.ToString("dd/MM/yyyy HH:mm"), headerFont, normalFont);
                    AddTableRow(table, "Mecánico:", $"{SelectedCita.vObjMecanico.f006_nombre} {SelectedCita.vObjMecanico.f006_apellido}", headerFont, normalFont);
                    AddTableRow(table, "Especialidad:", SelectedCita.vObjEspecialidad.f010_nombre, headerFont, normalFont);
                    AddTableRow(table, "Método de Pago:", PaymentMethod, headerFont, normalFont);

                    if (!string.IsNullOrEmpty(AdditionalDetails))
                    {
                        AddTableRow(table, "Detalles Adicionales:", AdditionalDetails, headerFont, normalFont);
                    }

                    pdfDoc.Add(table);

                    // Tabla de costos
                    PdfPTable costTable = new PdfPTable(2);
                    costTable.WidthPercentage = 100;
                    costTable.SetWidths(new float[] { 1f, 1f });
                    costTable.SpacingBefore = 20f;

                    AddTableRow(costTable, "Costo del Servicio:", $"${SelectedCita.vObjServicio.f014_valor:N2}", headerFont, normalFont);
                    AddTableRow(costTable, "Costo Adicional:", $"${AdditionalCost:N2}", headerFont, normalFont);
                    AddTableRow(costTable, "Total:", $"${TotalCost:N2}", headerFont, normalFont);

                    pdfDoc.Add(costTable);

                    // Añadir pie de página
                    Paragraph footer = new Paragraph("Gracias por confiar en nuestros servicios", normalFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    footer.SpacingBefore = 30f;
                    pdfDoc.Add(footer);

                    pdfDoc.Close();

                    var pdfBytes = memoryStream.ToArray();
                    return File(pdfBytes, "application/pdf", $"Factura_{DateTime.Now:yyyyMMdd}.pdf");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al generar la factura.";
                return RedirectToPage();
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
