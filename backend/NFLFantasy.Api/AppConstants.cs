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

        // NflPlayerService
        public const string ErrorMissingPlayerFields = "Todos los campos son obligatorios.";
        public const string ErrorNflTeamNotFound = "El equipo NFL seleccionado no existe.";
        public const string ErrorPlayerAlreadyExists = "Ya existe un jugador con ese nombre en el mismo equipo NFL.";
        public const string ErrorPositionNotFound = "La posición seleccionada no existe.";

        // LeagueService
        public const string ErrorInvalidLeagueName = "El nombre de la liga debe tener entre 1 y 100 caracteres.";
        public const string ErrorLeagueNameExists = "Ya existe una liga con ese nombre.";
        public const string ErrorInvalidTeamCount = "La cantidad de equipos no es válida.";
        public const string ErrorInvalidLeaguePassword = "La contraseña no cumple el formato requerido.";
        public const string ErrorNoActiveSeason = "No hay una temporada actual activa.";
        public const string ErrorTeamAliasExists = "El alias del equipo ya existe en el sistema. Intente con un nombre de equipo diferente.";
        public const string ErrorLeagueNotFound = "La liga no existe.";
        public const string ErrorLeagueInactive = "La liga no está activa.";
        public const string ErrorIncorrectPassword = "La contraseña es incorrecta.";
        public const string ErrorLeagueFull = "No hay cupos disponibles en la liga.";
        public const string ErrorAliasExistsInLeague = "El alias ya existe en la liga. Elige otro.";
        public const string ErrorTeamNameExistsInLeague = "El nombre de equipo ya existe en la liga. Elige otro.";
        public const string ErrorUserAlreadyInLeague = "Ya perteneces a esta liga.";

        // SeasonService
        public const string ErrorInvalidSeasonDates = "La fecha de fin debe ser posterior a la de inicio.";
        public const string ErrorPastDates = "Las fechas no pueden estar en el pasado.";
        public const string ErrorSeasonNameExists = "Ya existe una temporada con ese nombre.";
        public const string ErrorSeasonDateOverlap = "Las fechas se traslapan con otra temporada existente.";

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