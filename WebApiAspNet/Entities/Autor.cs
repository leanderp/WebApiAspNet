using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using WebApiAspNet.Helpers;

namespace WebApiAspNet.Entities
{
    public class Autor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "El Nombre del autor debe tener {1} carateres o menos")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public string Identificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public List<Libro> Libros { get; set; }
    }
}
