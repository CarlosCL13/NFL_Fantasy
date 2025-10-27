namespace NFLFantasy.Api
{
    // Constantes de la aplicación
    
    public static class AppConstants
    {
        // UserService
        public const string ErrorEmailAlreadyRegistered = "El correo electrónico proporcionado ya está registrado. Por favor, utiliza otro correo o intenta recuperar tu contraseña.";
        public const string ErrorAliasInUse = "El alias seleccionado ya está en uso. Elige un alias diferente para tu cuenta.";
        public const string ErrorMissingUserFields = "Faltan campos obligatorios: asegúrate de completar nombre, correo, alias y contraseña.";
        public const string ErrorInvalidCredentials = "El correo o la contraseña ingresados son incorrectos. Verifica tus datos e inténtalo de nuevo.";
        public const string ErrorAccountLocked = "Tu cuenta ha sido bloqueada tras varios intentos fallidos de inicio de sesión. Por favor, contacta al administrador para desbloquearla.";

        // NflTeamService
        public const string ErrorNflTeamNameExists = "Ya existe un equipo NFL registrado con el nombre proporcionado. Elige un nombre diferente.";
        public const string ErrorMissingNflTeamFields = "Debes proporcionar nombre, ciudad, imagen y miniatura para crear un equipo NFL.";

        // Imagen
        public static readonly string[] AllowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
        public const long MaxImageFileSize = 2 * 1024 * 1024; // 2MB
        public const string DefaultProfileImage = "default.png";
        public const string UsersImageFolder = "wwwroot/images/users";
        public const string NflTeamsImageFolder = "wwwroot/images/nflteams";

        // Mensajes de error
        public const string ErrorInvalidImageFormat = "Formato de imagen no permitido. Solo se aceptan archivos .jpg, .jpeg y .png.";
        public const string ErrorImageTooLarge = "La imagen excede el tamaño máximo permitido de 2MB.";
        public const string ErrorProfileImageTooLarge = "La imagen de perfil excede el tamaño máximo permitido de 2MB.";
        public const string ErrorProfileImageFormat = "Formato de imagen de perfil no permitido. Solo se aceptan archivos .jpg, .jpeg y .png.";
        public const string ErrorRequiredImage = "Debes adjuntar una imagen válida para el equipo NFL. Formatos permitidos: .jpg, .png. Tamaño máximo: 2MB.";
        public const string ErrorInvalidRegisterData = "Datos de registro inválidos.";
        public const string ErrorInvalidTeamData = "Datos inválidos para crear el equipo NFL.";
    }
}