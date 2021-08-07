using System.Linq;
using Aplicacion.Cursos.DTO;
using AutoMapper;
using Dominio;

namespace Aplicacion
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Curso, CursoDTO>()
            .ForMember(des => des.Instructores, src => src.MapFrom(y => y.InstructoresLink.Select(a => a.Instructor).ToList()))
            .ForMember(des => des.Comentarios, src => src.MapFrom(z => z.ComentarioLista))
            .ForMember(des => des.Precio, src => src.MapFrom(z => z.PrecioPromocion));
            CreateMap<CursoInstructor, CursoInstructorDTO>();
            CreateMap<Instructor, InstructorDTO>();
            CreateMap<Comentario, ComentarioDTO>();
            CreateMap<Precio, PrecioDTO>();
        }
    }
}