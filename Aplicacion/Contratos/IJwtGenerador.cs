using Dominio;

namespace Aplicacion.Contratos
{
    public interface IJwtGenerador
    {
         string CreateToken(Usuario usuario);
    }
}