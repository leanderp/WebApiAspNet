using System;
using System.ComponentModel.DataAnnotations;

using WebApiAspNet.Helpers;

namespace WebApiAspNet.Models
{
    public class AutorCreacionDTO
    {
        [Required]
        [StringLength(50, ErrorMessage = "El Nombre del autor debe tener {1} carateres o menos")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
    }
}
