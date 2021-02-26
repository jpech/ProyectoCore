using System;

namespace Aplicacion.Cursos.DTO
{
    public class InstructorDTO
    {
        public Guid InstructorId { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public string Grado { get; set; }

        public byte[] FotoPerfil { get; set; }
    }
}