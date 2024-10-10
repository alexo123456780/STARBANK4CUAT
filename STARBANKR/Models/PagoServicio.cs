using System.ComponentModel.DataAnnotations;

namespace STARBANKR.Models
{
    public class PagoServicio
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public int ServicioId { get; set; }
        public double Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public virtual Cuenta Cuenta { get; set; }
        public virtual Servicio Servicio { get; set; }
    }
}