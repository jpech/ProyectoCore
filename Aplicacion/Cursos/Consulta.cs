using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Cursos.DTO;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Consulta
    {
        public class ListaCursos : IRequest<List<CursoDTO>> {}

        public class Manejador : IRequestHandler<ListaCursos, List<CursoDTO>>
        {
            private readonly CursosOnlineContext context;

            private readonly IMapper _mapper;

            public Manejador(CursosOnlineContext _context, IMapper mapper)
            {
                context = _context;
                _mapper = mapper;
            }
            public async Task<List<CursoDTO>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                var cursos = await context.Curso
                             .Include(x => x.InstructoresLink)
                             .ThenInclude(x => x.Instructor).ToListAsync();

                var cursosDto = _mapper.Map<List<Curso>, List<CursoDTO>>(cursos);

                return cursosDto;
            }
        }
    }
}