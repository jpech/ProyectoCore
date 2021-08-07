using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest 
        {
            public Guid CursoId { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext context;
            public Manejador(CursosOnlineContext _context)
            {
                context = _context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var instructoresBd = context.CursoInstructor.Where(w => w.CursoId == request.CursoId).ToList();
                if(instructoresBd != null && instructoresBd.Any()){
                    foreach(var instructorBd in instructoresBd){
                        context.CursoInstructor.Remove(instructorBd);
                    }
                }

                var curso = await context.Curso.FindAsync(request.CursoId);
                if(curso == null)
                {
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "No se encontró el curso."});
                }

                context.Remove(curso);
                var valor = await context.SaveChangesAsync();
                if(valor > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se pudo eliminar el curso.");
            }
        }
    }
}