using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        public class Ejecuta : IRequest<UsuarioData>{
            public string Nombre { get; set; } 

            public string Apellidos { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }

            public string UserName{ get; set; }
        }

        public class EjecutaValidador : AbstractValidator<Ejecuta>{
            public EjecutaValidador()
            {
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.UserName).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext _context;

            private readonly UserManager<Usuario> _userManager;

            private readonly IJwtGenerador _jwtGenerador;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var existe = await _context.Users.Where(w => w.Email == request.Email).AnyAsync();
                if(existe){
                    throw new ManejadorException(HttpStatusCode.BadRequest, new {mensaje = "El email ingresado ya existe."});
                }

                var existeUsername = await _context.Users.Where(w => w.UserName == request.UserName).AnyAsync();
                if(existeUsername){
                    throw new ManejadorException(HttpStatusCode.BadRequest, new {mensaje = "Ya existe un usuario con este username."});
                }

                var usuario = new Usuario{
                    NombreCompleto = string.Format("{0} {1}", request.Nombre, request.Apellidos),
                    Email = request.Email,
                    UserName = request.UserName
                };

                var resultado = await _userManager.CreateAsync(usuario, request.Password);
                if(resultado.Succeeded){
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        Token = _jwtGenerador.CreateToken(usuario),
                        UserName = usuario.UserName,
                        Email = usuario.Email
                    };
                }

                throw new Exception("No se pudo agregar al nuevo usuario");

            }
        }
    }
}