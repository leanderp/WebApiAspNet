using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApiAspNet.Models
{
    public class AutorDTO : Recurso
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public List<LibroDTO> Libros { get; set; }
    }
}
