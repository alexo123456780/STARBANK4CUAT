namespace STARBANKR.Models
{
    public class VerificacionPinDTO
    {
        public string NumeroTarjeta { get; set; }
        public string Pin { get; set; }
    }

    public class TransaccionDTO
    {
        public string NumeroTarjeta { get; set; }
        public double Monto { get; set; }
    }

    public class PagoTarjetaCreditoDTO
    {
        public string NumeroTarjeta { get; set; }
        public double Monto { get; set; }
        public bool EsAbonoCapital { get; set; }
    }

    public class PagoServicioDTO
    {
        public string NumeroTarjeta { get; set; }
        public string TipoServicio { get; set; }
        public double Monto { get; set; }
    }

    public class CreditoEducativoDTO
    {
        public string NumeroTarjeta { get; set; }
        public double Monto { get; set; }
        public int Plazo { get; set; }
    }

    public class ComprobanteDTO
    {
        public string NumeroTarjeta { get; set; }
        public string TipoOperacion { get; set; }
        public double Monto { get; set; }
    }
}