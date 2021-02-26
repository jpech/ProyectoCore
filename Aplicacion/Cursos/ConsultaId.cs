using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Cursos.DTO;
using Aplicacion.ManejadorError;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursoUnico : IRequest<CursoDTO> 
        {
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<CursoUnico, CursoDTO>
        {
            private readonly CursosOnlineContext context;

            private readonly IMapper _mapper;

            public Manejador(CursosOnlineContext _context, IMapper mapper)
            {
                context = _context;
                _mapper = mapper;
            }

            public async Task<CursoDTO> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                var curso = await context.Curso
                            .Include(x => x.InstructoresLink).ThenInclude(x => x.Instructor)
                            .FirstOrDefaultAsync(x => x.CursoId == request.Id);

                if(curso == null)
                {
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "No se encontró el curso."});
                }

                var cursoDto = _mapper.Map<Curso, CursoDTO>(curso);

                return cursoDto;
            }
        }
    }
}