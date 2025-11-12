using BCrypt.Net;

namespace NFLFantasy.Api.Utils
{
    /// <summary>
    /// Utilidad para operaciones de hashing y verificación de contraseñas.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Genera un hash seguro de la contraseña usando BCrypt.
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifica si la contraseña coincide con el hash almacenado.
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
