using System;

namespace Aplicacion.Cursos.DTO
{
    public class ComentarioDTO
    {
        public Guid ComentarioId { get; set; }

        public string Alumno { get; set; }

        public int Puntaje { get; set; }

        public string ComentarioTexto { get; set; }

        public Guid CursoId { get; set; }
    }
}