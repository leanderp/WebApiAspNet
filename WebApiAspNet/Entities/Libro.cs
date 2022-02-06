using System.ComponentModel.DataAnnotations;

using WebApiAspNet.Helpers;

namespace WebApiAspNet.Entities
{
    public class Libro
    {
        public int Id { get; set; }
        [Required]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }
        [Required]
        public int AutorId { get; set; }
        public Autor Autor { get; set; }
    }
}
