using System.ComponentModel.DataAnnotations;

namespace STARBANKR.Models
{
    public class Comprobante
    {
        public int Id { get; set; }
        public int TransaccionId { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public virtual Transaccion Transaccion { get; set; }
    }
}