using System.ComponentModel.DataAnnotations;

namespace STARBANKR.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NumeroReferencia { get; set; }
    }
}