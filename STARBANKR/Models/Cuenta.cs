using System.ComponentModel.DataAnnotations;

namespace STARBANKR.Models
{
    public class Cuenta
    {
        [Key]
        public int Id { get; set; }
        public string NumeroTarjeta { get; set; }
        public string NombreUsuario { get; set; }
        public string Pin { get; set; }
        public double Saldo { get; set; }
        public int IntentosFallidos { get; set; }
        public TipoCuenta TipoCuenta { get; set; }
        public double? LimiteCredito { get; set; }
        public DateTime? FechaCorte { get; set; }
        public DateTime? FechaPago { get; set; }
    }

    public enum TipoCuenta
    {
        Debito,
        Credito
    }
}