using System.ComponentModel.DataAnnotations;

namespace STARBANKR.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int CuentaId { get; set; }
        public string TipoOperacion { get; set; }
        public double Monto { get; set; }
        public DateTime Fecha { get; set; }
        public virtual Cuenta Cuenta { get; set; }
    }
}