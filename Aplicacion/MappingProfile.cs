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
            .ForMember(des => des.Instructores, src => src.MapFrom(y => y.InstructoresLink.Select(a => a.Instructor).ToList()));
            CreateMap<CursoInstructor, CursoInstructorDTO>();
            CreateMap<Instructor, InstructorDTO>();
        }
    }
}