using System.Net.Mail;
using System.Net;

namespace mecanico_plus.Pages.Backend.logicaNegocio
{
    public class GestionCorreos
    {

        public string enviarCorreo(string destinatario, string asunto, string body, byte[] archivoAdjunto, string nombreArchivo)
        {
            string emailOrigen = "soporte@codex-ai-tech.com";
            string claveEmail = "fvshfnjyelhbxjzg";
            string resultado = string.Empty;

            // Formateamos el cuerpo del correo como HTML y aplicamos negrillas con la etiqueta <b>
            string mensaje = $@"
<p>Estimado usuario,</p>
<br/>
<p>{body}</p>
<br/>
<p>Gracias por su atención.</p>
<br/>
<p><b>Cordialmente,</b><br/>Equipo de soporte y telecomunicaciones</p>
<p><b>Codex-AI</b><br/>Contacto | <a href='https://www.codex-ai-tech.com/contacto' target='_blank'>https://www.codex-ai-tech.com/contacto</a></p>
<hr/>
<p style='font-size: 12px; color: gray;'>Este correo electrónico y cualquier archivo adjunto pueden contener información confidencial. Si no es el destinatario previsto, por favor notifíquelo al remitente y elimine el mensaje.</p>";

            MailMessage message = null;
            SmtpClient smtpClient = null;
            MemoryStream ms = null;

            try
            {
                message = new MailMessage
                {
                    From = new MailAddress(emailOrigen),
                    Subject = asunto,
                    Body = mensaje,
                    IsBodyHtml = true // Importante para que se interprete como HTML
                };

                // Agregar los destinatarios, separados por punto y coma
                var destinatarios = destinatario.Split(';');
                foreach (var email in destinatarios)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        message.To.Add(email.Trim());
                    }
                }

                // Adjuntar el archivo si está presente
                if (archivoAdjunto != null && archivoAdjunto.Length > 0)
                {
                    ms = new MemoryStream(archivoAdjunto);
                    Attachment adjunto = new Attachment(ms, nombreArchivo);
                    message.Attachments.Add(adjunto);
                }

                smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(emailOrigen, claveEmail),
                    EnableSsl = true
                };

                smtpClient.Send(message);

                resultado = "El correo se ha enviado exitosamente.";
                Console.WriteLine("Correo enviado correctamente.");
            }
            catch (Exception ex)
            {
                resultado = "Error al enviar el correo: " + ex.ToString();
                Console.WriteLine("Error: " + ex.ToString());
            }
            finally
            {
                // Dispose of resources
                if (ms != null)
                    ms.Dispose();

                if (message != null)
                    message.Dispose();

                if (smtpClient != null)
                    smtpClient.Dispose();
            }

            return resultado;
        }
    }
}

