using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using NFLFantasy.Api.Services;
using NFLFantasy.Api.DTO;
using NFLFantasy.Api.DataAccessLayer.Repositories;
using NFLFantasy.Api.DataAccessLayer.StorageManagement;
using NFLFantasy.Api.DataAccessLayer.FileManagement;
using NFLFantasy.Api.Validators;
using NFLFantasy.Api.Data;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NFLFantasy.Api.Tests
{
    public class NflPlayerBulkServiceTests
    {
        private readonly Mock<NflPlayerService> _playerServiceMock = new(MockBehavior.Strict, null!, null!, null!);
        private readonly Mock<IJsonFileHandler> _jsonFileHandlerMock = new();
        private readonly Mock<IDirectoryManager> _directoryManagerMock = new();
        private readonly Mock<INflPlayerRepository> _repositoryMock = new();
        private readonly Mock<FantasyContext> _contextMock = new(new DbContextOptions<FantasyContext>());
        private readonly Mock<DatabaseFacade> _databaseMock;
        private readonly Mock<IDbContextTransaction> _transactionMock = new();

        public NflPlayerBulkServiceTests()
        {
            _databaseMock = new Mock<DatabaseFacade>(_contextMock.Object);
            _contextMock.Setup(c => c.Database).Returns(_databaseMock.Object);
            _databaseMock.Setup(d => d.BeginTransaction()).Returns(_transactionMock.Object);
        }
        
        private NflPlayerBulkService CreateService(NflPlayerValidator? validator = null)
        {
            return new NflPlayerBulkService(
                _playerServiceMock.Object,
                _jsonFileHandlerMock.Object,
                _contextMock.Object,
                _directoryManagerMock.Object,
                validator ?? new NflPlayerValidator(null!),
                _repositoryMock.Object
            );
        }

        /// <summary>
        /// Prueba que verifica que el método HandleBulkUploadAsync devuelve un error cuando los datos del jugador son inválidos.
        /// </summary>
        [Fact]
        public async Task HandleBulkUploadAsync_ReturnsError_WhenPlayerDataIsInvalid()
        {
            // ARRANGE: Preparar el escenario de prueba
            // Este test valida que cuando el equipo NFL no existe, se rechaza el jugador
            
            // 1. Crear directorios temporales para el test
            var uploadsDir = System.IO.Path.Combine("wwwroot", "uploads");
            var processedDir = System.IO.Path.Combine("wwwroot", "processed");
            System.IO.Directory.CreateDirectory(uploadsDir);
            System.IO.Directory.CreateDirectory(processedDir);
            
            // 2. Crear el validador de jugadores (sin repositorio mock en el constructor)
            var validator = new NflPlayerValidator(null!);

            // 3. Configurar el mock del repositorio para simular que el equipo NFL NO existe
            // Esto hará que la validación falle
            _repositoryMock.Setup(r => r.NflTeamExists(It.IsAny<int>())).Returns(false);

            // 4. Crear un archivo JSON mock con datos de prueba
            // El JSON contiene un jugador válido en formato, pero con un equipo que no existe
            var jsonContent = "[{\"Name\":\"Test\",\"PositionId\":1,\"NflTeamId\":1,\"ImagePath\":\"img.png\"}]";
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var stream = new System.IO.MemoryStream(jsonBytes);
            
            // 5. Configurar el mock de IFormFile para simular un archivo subido
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(jsonBytes.Length);
            fileMock.Setup(f => f.FileName).Returns("jugadores.json");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            // Configurar CopyToAsync para que copie el contenido del stream al destino
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<System.IO.Stream>(), default)).Returns<System.IO.Stream, System.Threading.CancellationToken>((target, token) => {
                stream.Position = 0; // Resetear posición del stream
                return stream.CopyToAsync(target, token);
            });

            // 6. Configurar mocks del DirectoryManager para simular gestión de directorios
            var jsonUploadsFolder = uploadsDir;
            var jsonProcessedFolder = processedDir;
            _directoryManagerMock.Setup(d => d.EnsureAllNflPlayersDirectoriesExist());
            _directoryManagerMock.Setup(d => d.GetNflPlayersUploadsPath()).Returns(jsonUploadsFolder);
            _directoryManagerMock.Setup(d => d.GetNflPlayersProcessedPath()).Returns(jsonProcessedFolder);
            _directoryManagerMock.Setup(d => d.GenerateUniqueFileName(It.IsAny<string>(), ".json")).Returns("jugadores_test.json");

            // 7. Configurar rutas de archivos y mock del manejador de JSON
            var testJsonPath = System.IO.Path.Combine(uploadsDir, "jugadores_test.json");
            var processedJsonPath = System.IO.Path.Combine(processedDir, "jugadores_test.json");
            // El tercer parámetro 'true' indica que hubo errores al procesar
            _jsonFileHandlerMock.Setup(j => j.MoveToProcessedFolder(It.IsAny<string>(), It.IsAny<string>(), true)).Returns((true, processedJsonPath, null));

            // 8. Crear la instancia del servicio con el validador configurado
            var service = CreateService(validator);

            try
            {
                // ACT: Ejecutar el método que estamos probando
                var result = await service.HandleBulkUploadAsync(fileMock.Object);

                // ASSERT: Verificar que el resultado es el esperado
                // 9. Verificar que el procesamiento falló (porque el equipo no existe)
                result.Success.Should().BeFalse();
                
                // 10. Verificar que se generó el mensaje de error correcto
                try
                {
                    result.Errors.Should().ContainSingle(e => e.Contains("El equipo NFL seleccionado no existe"));
                }
                catch
                {
                    // Si la aserción falla, imprimir los errores reales para debugging
                    System.Console.WriteLine("Errores reales: " + string.Join(" | ", result.Errors));
                    throw;
                }
            }
            finally
            {
                // CLEANUP: Limpiar archivos temporales creados durante el test
                // 11. Eliminar archivos de prueba para no contaminar otros tests
                if (System.IO.File.Exists(testJsonPath))
                    System.IO.File.Delete(testJsonPath);
                if (System.IO.File.Exists(processedJsonPath))
                    System.IO.File.Delete(processedJsonPath);
            }
        }

        /// <summary>
        /// Prueba que verifica que el método HandleBulkUploadAsync devuelve un error cuando el archivo JSON es inválido.
        /// </summary>
        [Fact]
        public async Task HandleBulkUploadAsync_ReturnsError_WhenJsonIsEmpty()
        {
            // ARRANGE: Preparar escenario con archivo JSON vacío
            // Este test valida que se rechaza un archivo sin contenido
            
            // 1. Crear directorios temporales
            var uploadsDir = System.IO.Path.Combine("wwwroot", "uploads");
            var processedDir = System.IO.Path.Combine("wwwroot", "processed");
            System.IO.Directory.CreateDirectory(uploadsDir);
            System.IO.Directory.CreateDirectory(processedDir);
            
            // 2. Crear validador
            var validator = new NflPlayerValidator(null!);

            // 3. Crear mock de archivo con contenido VACÍO
            var fileMock = new Mock<IFormFile>();
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(""); // String vacío
            var stream = new System.IO.MemoryStream(jsonBytes);
            
            // 4. Configurar comportamiento del mock de IFormFile
            // 4. Configurar comportamiento del mock de IFormFile
            fileMock.Setup(f => f.Length).Returns(jsonBytes.Length);
            fileMock.Setup(f => f.FileName).Returns("jugadores.json");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<System.IO.Stream>(), default)).Returns<System.IO.Stream, System.Threading.CancellationToken>((target, token) => {
                stream.Position = 0;
                return stream.CopyToAsync(target, token);
            });

            // 5. Configurar mocks de gestión de directorios
            var jsonUploadsFolder = uploadsDir;
            var jsonProcessedFolder = processedDir;
            _directoryManagerMock.Setup(d => d.EnsureAllNflPlayersDirectoriesExist());
            _directoryManagerMock.Setup(d => d.GetNflPlayersUploadsPath()).Returns(jsonUploadsFolder);
            _directoryManagerMock.Setup(d => d.GetNflPlayersProcessedPath()).Returns(jsonProcessedFolder);
            _directoryManagerMock.Setup(d => d.GenerateUniqueFileName(It.IsAny<string>(), ".json")).Returns("jugadores_test.json");

            // 6. Configurar rutas y manejador de archivos JSON
            var testJsonPath = System.IO.Path.Combine(uploadsDir, "jugadores_test.json");
            var processedJsonPath = System.IO.Path.Combine(processedDir, "jugadores_test.json");
            _jsonFileHandlerMock.Setup(j => j.MoveToProcessedFolder(It.IsAny<string>(), It.IsAny<string>(), true)).Returns((true, processedJsonPath, null));

            // 7. Crear servicio
            var service = CreateService(validator);
            try
            {
                // ACT: Ejecutar el método con archivo vacío
                var result = await service.HandleBulkUploadAsync(fileMock.Object);
                
                // ASSERT: Verificar que se rechaza el archivo vacío
                // 8. Debe fallar porque no hay datos
                result.Success.Should().BeFalse();
                
                // 9. El error puede ser cualquiera de estos mensajes relacionados con archivo vacío
                try
                {
                    result.Errors.Should().Contain(e =>
                        e.Contains("El archivo no contiene datos de jugadores") ||
                        e.Contains("El archivo no tiene formato JSON válido") ||
                        e.Contains("Debe adjuntar un archivo JSON.")
                    );
                }
                catch
                {
                    // Mostrar errores reales si la aserción falla
                    System.Console.WriteLine("Errores reales: " + string.Join(" | ", result.Errors));
                    throw;
                }
            }
            finally
            {
                // CLEANUP: Limpiar archivos temporales
                if (System.IO.File.Exists(testJsonPath))
                    System.IO.File.Delete(testJsonPath);
                if (System.IO.File.Exists(processedJsonPath))
                    System.IO.File.Delete(processedJsonPath);
            }
        }

        /// <summary>
        /// Prueba que verifica que el método HandleBulkUploadAsync devuelve un error cuando el JSON es inválido.
        /// </summary>
        [Fact]
        public async Task HandleBulkUploadAsync_ReturnsError_WhenJsonIsInvalid()
        {
            // ARRANGE: Preparar escenario con JSON mal formado
            // Este test valida que se rechaza un archivo con sintaxis JSON inválida
            
            // 1. Crear directorios temporales
            var uploadsDir = System.IO.Path.Combine("wwwroot", "uploads");
            var processedDir = System.IO.Path.Combine("wwwroot", "processed");
            System.IO.Directory.CreateDirectory(uploadsDir);
            System.IO.Directory.CreateDirectory(processedDir);
            
            // 2. Crear validador
            var validator = new NflPlayerValidator(null!);

            // 3. Crear mock de archivo con JSON INVÁLIDO (sintaxis incorrecta)
            var fileMock = new Mock<IFormFile>();
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes("{not valid json}"); // JSON mal formado
            var stream = new System.IO.MemoryStream(jsonBytes);
            
            // 4. Configurar comportamiento del mock
            fileMock.Setup(f => f.Length).Returns(jsonBytes.Length);
            fileMock.Setup(f => f.FileName).Returns("jugadores.json");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<System.IO.Stream>(), default)).Returns<System.IO.Stream, System.Threading.CancellationToken>((target, token) => {
                stream.Position = 0;
                return stream.CopyToAsync(target, token);
            });

            // 5. Configurar mocks de gestión de directorios
            var jsonUploadsFolder = uploadsDir;
            var jsonProcessedFolder = processedDir;
            _directoryManagerMock.Setup(d => d.EnsureAllNflPlayersDirectoriesExist());
            _directoryManagerMock.Setup(d => d.GetNflPlayersUploadsPath()).Returns(jsonUploadsFolder);
            _directoryManagerMock.Setup(d => d.GetNflPlayersProcessedPath()).Returns(jsonProcessedFolder);
            _directoryManagerMock.Setup(d => d.GenerateUniqueFileName(It.IsAny<string>(), ".json")).Returns("jugadores_test.json");

            // 6. Configurar rutas y manejador de archivos
            var testJsonPath = System.IO.Path.Combine(uploadsDir, "jugadores_test.json");
            var processedJsonPath = System.IO.Path.Combine(processedDir, "jugadores_test.json");
            _jsonFileHandlerMock.Setup(j => j.MoveToProcessedFolder(It.IsAny<string>(), It.IsAny<string>(), true)).Returns((true, processedJsonPath, null));

            // 7. Crear servicio
            var service = CreateService(validator);
            try
            {
                // ACT: Intentar procesar el JSON inválido
                var result = await service.HandleBulkUploadAsync(fileMock.Object);
                
                // ASSERT: Verificar que se rechaza por formato inválido
                // 8. Debe fallar porque el JSON no es parseable
                result.Success.Should().BeFalse();
                
                // 9. Verificar que el mensaje de error es sobre formato JSON inválido
                try
                {
                    result.Errors.Should().Contain(e => e.Contains("El archivo no tiene formato JSON válido"));
                }
                catch
                {
                    // Mostrar errores reales para debugging
                    System.Console.WriteLine("Errores reales: " + string.Join(" | ", result.Errors));
                    throw;
                }
            }
            finally
            {
                // CLEANUP: Limpiar archivos temporales
                if (System.IO.File.Exists(testJsonPath))
                    System.IO.File.Delete(testJsonPath);
                if (System.IO.File.Exists(processedJsonPath))
                    System.IO.File.Delete(processedJsonPath);
            }
        }

        /// <summary>
        /// Prueba que verifica que el método HandleBulkUploadAsync devuelve éxito cuando todos los datos del jugador son válidos.
        /// </summary>
        [Fact]
        public async Task HandleBulkUploadAsync_ReturnsSuccess_WhenAllDataIsValid()
        {
            // ARRANGE: Preparar escenario con datos completamente válidos
            // Este test valida el caso de éxito: todo funciona correctamente
            
            // 1. Crear directorios temporales
            var uploadsDir = System.IO.Path.Combine("wwwroot", "uploads");
            var processedDir = System.IO.Path.Combine("wwwroot", "processed");
            System.IO.Directory.CreateDirectory(uploadsDir);
            System.IO.Directory.CreateDirectory(processedDir);
            
            // 2. Crear validador
            var validator = new NflPlayerValidator(null!);

            // 3. Configurar mock del DirectoryManager para retornar ruta de imágenes
            _directoryManagerMock.Setup(d => d.GetNflPlayersImagesPath()).Returns(uploadsDir);

            // 4. Configurar mocks del repositorio para que las validaciones PASEN
            // El equipo SÍ existe
            _repositoryMock.Setup(r => r.NflTeamExists(It.IsAny<int>())).Returns(true);
            // La posición SÍ existe
            _repositoryMock.Setup(r => r.PositionExists(It.IsAny<int>())).Returns(true);

            // 5. Configurar mock del NflPlayerService para simular creación exitosa
            // Retorna (true, null) indicando éxito sin errores
            _playerServiceMock.Setup(ps => ps.CreateNflPlayerInternalAsync(
                It.IsAny<NflPlayerCreateDto>(),
                It.IsAny<byte[]>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).ReturnsAsync((true, (string?)null));

            // 6. Crear un archivo JSON válido con datos de jugador correctos
            var jsonContent = "[{\"Name\":\"Test\",\"PositionId\":1,\"NflTeamId\":1,\"ImagePath\":\"img.png\"}]";
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            var stream = new System.IO.MemoryStream(jsonBytes);
            
            // 7. Configurar mock del archivo subido
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(jsonBytes.Length);
            fileMock.Setup(f => f.FileName).Returns("jugadores.json");
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<System.IO.Stream>(), default)).Returns<System.IO.Stream, System.Threading.CancellationToken>((target, token) => {
                stream.Position = 0;
                return stream.CopyToAsync(target, token);
            });

            // 8. Configurar mocks de gestión de directorios y archivos
            var jsonUploadsFolder = uploadsDir;
            var jsonProcessedFolder = processedDir;
            _directoryManagerMock.Setup(d => d.EnsureAllNflPlayersDirectoriesExist());
            _directoryManagerMock.Setup(d => d.GetNflPlayersUploadsPath()).Returns(jsonUploadsFolder);
            _directoryManagerMock.Setup(d => d.GetNflPlayersProcessedPath()).Returns(jsonProcessedFolder);
            _directoryManagerMock.Setup(d => d.GenerateUniqueFileName(It.IsAny<string>(), ".json")).Returns("jugadores_test.json");
            
            // 9. Configurar rutas de archivos
            var testJsonPath = System.IO.Path.Combine(uploadsDir, "jugadores_test.json");
            var processedJsonPath = System.IO.Path.Combine(processedDir, "jugadores_test.json");
            // El tercer parámetro 'false' indica que NO hubo errores (éxito)
            _jsonFileHandlerMock.Setup(j => j.MoveToProcessedFolder(It.IsAny<string>(), It.IsAny<string>(), false)).Returns((true, processedJsonPath, null));

            // 10. Crear archivo de imagen fake para que no falle la validación de imagen
            System.IO.File.WriteAllText("img.png", "fake image");

            // 11. Crear servicio
            var service = CreateService(validator);
            try
            {
                // ACT: Ejecutar el procesamiento con datos válidos
                var result = await service.HandleBulkUploadAsync(fileMock.Object);
                
                // ASSERT: Verificar que el procesamiento fue exitoso
                // 12. El resultado debe ser exitoso
                result.Success.Should().BeTrue();
                // 13. Debe haberse creado 1 jugador
                result.CreatedCount.Should().Be(1);
                // 14. El mensaje de éxito debe contener el nombre del jugador
                result.SuccessMessages.Should().Contain(msg => msg.Contains("Test"));
            }
            finally
            {
                // CLEANUP: Limpiar todos los archivos temporales creados
                if (System.IO.File.Exists(testJsonPath))
                    System.IO.File.Delete(testJsonPath);
                if (System.IO.File.Exists(processedJsonPath))
                    System.IO.File.Delete(processedJsonPath);
                if (System.IO.File.Exists("img.png"))
                    System.IO.File.Delete("img.png");
            }
        }
    }
}
