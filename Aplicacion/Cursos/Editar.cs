using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        public class Ejecuta : IRequest 
        {
            public Guid CursoId { get; set; }
            public string Titulo { get; set; }

            public string Descripcion { get; set; }

            public DateTime? FechaPublicacion { get; set; }

            public List<Guid> ListaInstructor { get; set; }
        }

        public class EjecutaValidation : AbstractValidator<Ejecuta>
        {
            public EjecutaValidation()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
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
                var curso = await context.Curso.FindAsync(request.CursoId);

                if(curso == null)
                {
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "No se encontró el curso."});
                }

                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;

                if(request.ListaInstructor != null && request.ListaInstructor.Any()){
                    
                    var instructoresBD = context.CursoInstructor.Where(w => w.CursoId == request.CursoId).ToList();
                    foreach(var _instructor in instructoresBD){
                        context.CursoInstructor.Remove(_instructor);
                    }

                    foreach(var _instructorId in request.ListaInstructor){
                        var nuevoInstructor = new CursoInstructor{
                            CursoId = request.CursoId,
                            InstructorId = _instructorId
                        };

                        context.CursoInstructor.Add(nuevoInstructor);
                    }
                }

                var valor = await context.SaveChangesAsync();
                
                if(valor > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se pudo insertar el curso.");
            }
        }
    }
}