namespace mecanico_plus.Pages.Login
{
    public class ModeloPagoData
    {
        public string Usuario { get; set; }
        public string Factura { get; set; }
        public int Valor { get; set; }
        public string DescripcionFactura { get; set; }
        public string TokenSeguridad { get; set; }
        public string DocumentoComprador { get; set; }
        public string TipoDocumento { get; set; }
        public string NombreComprador { get; set; }
        public string ApellidoComprador { get; set; }
        public string CorreoComprador { get; set; }
    }
}
