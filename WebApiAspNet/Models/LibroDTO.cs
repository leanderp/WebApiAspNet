using System.ComponentModel.DataAnnotations;

namespace WebApiAspNet.Models
{
    public class LibroDTO : Recurso
    {
        public int Id { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public int AutorId { get; set; }
        public AutorDTO Autor { get; set; }
    }
}
