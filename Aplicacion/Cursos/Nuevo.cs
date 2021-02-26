using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest 
        {
            public string Titulo { get; set; }

            //[Required(ErrorMessage = "Por favor ingrese la descipción.")]
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
                RuleFor(x => x.ListaInstructor).NotEmpty();
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
                Guid _cursoId = Guid.NewGuid();
                var curso = new Curso
                {
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion
                };

                if(request.ListaInstructor != null){
                    foreach (var item in request.ListaInstructor){
                        var cursoInstructor = new CursoInstructor{
                            CursoId = curso.CursoId,
                            InstructorId = item
                        };

                        context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                context.Curso.Add(curso);
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