using AutoMapper;

using WebApiAspNet.Entities;

namespace WebApiAspNet.Models
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Autor, AutorDTO>();
            CreateMap<AutorCreacionDTO, Autor>();
            CreateMap<Autor, AutorCreacionDTO>();

            CreateMap<Libro, LibroDTO>();
            CreateMap<LibroDTO, Libro>();
        }
    }
}
