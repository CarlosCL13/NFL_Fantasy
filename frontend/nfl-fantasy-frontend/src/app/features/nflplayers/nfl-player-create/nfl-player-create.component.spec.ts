import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NflPlayerCreateComponent } from './nfl-player-create.component';

// Servicios y Router
import { NflPlayerService } from '../../../core/services/nflplayer.service';
import { NflTeamService } from '../../../core/services/nflteam.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';

// RxJS
import { of, throwError } from 'rxjs';

describe('NflPlayerCreateComponent - Bulk Upload', () => {
  let component: NflPlayerCreateComponent;
  let fixture: ComponentFixture<NflPlayerCreateComponent>;

  // 1. Declaramos los espías (Mocks)
  let mockPlayerService: jasmine.SpyObj<NflPlayerService>;
  let mockTeamService: jasmine.SpyObj<NflTeamService>;
  let mockErrorHandler: jasmine.SpyObj<ErrorHandlerService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    // 2. Creamos los objetos espía con los métodos exactos que usa el componente
    mockPlayerService = jasmine.createSpyObj('NflPlayerService', [
      'bulkUpload',
      'getPositions',
      'createPlayer',
    ]);
    mockTeamService = jasmine.createSpyObj('NflTeamService', ['getTeams']);
    mockErrorHandler = jasmine.createSpyObj('ErrorHandlerService', ['handleError']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    // 3. Configuración inicial de las respuestas (Happy Path por defecto)
    mockPlayerService.getPositions.and.returnValue(of([]));
    mockTeamService.getTeams.and.returnValue(of([]));

    // Configuración del módulo de pruebas
    await TestBed.configureTestingModule({
      // Importamos el componente porque es Standalone
      imports: [NflPlayerCreateComponent, ReactiveFormsModule],
      // Proveemos los Mocks en lugar de los servicios reales
      providers: [
        FormBuilder,
        { provide: NflPlayerService, useValue: mockPlayerService },
        { provide: NflTeamService, useValue: mockTeamService },
        { provide: ErrorHandlerService, useValue: mockErrorHandler },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    // Creación del componente
    fixture = TestBed.createComponent(NflPlayerCreateComponent);
    component = fixture.componentInstance;

    // Ejecuta ngOnInit
    fixture.detectChanges();
  });

  // --------------------------------------------------------------------------
  // BLOQUE 1: PRUEBAS BÁSICAS Y DE CICLO DE VIDA
  // --------------------------------------------------------------------------

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe inicializar las variables de bulk upload correctamente', () => {
    expect(component.bulkFile).toBeUndefined();
    expect(component.bulkMessage).toBe('');
    expect(component.bulkError).toBe('');
  });

  // --------------------------------------------------------------------------
  // BLOQUE 2: SUITE 1 - VALIDACIONES PREVIAS
  // --------------------------------------------------------------------------

  describe('SUITE 1: Validaciones Previas', () => {
    it('Test 1.1: Debe mostrar error cuando no hay archivo seleccionado', () => {
      // Arrange
      component.bulkFile = undefined;

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toBe('Debes seleccionar un archivo JSON antes de subirlo.');
      expect(component.bulkMessage).toBe('');
    });

    it('Test 1.2: Debe limpiar mensajes previos al seleccionar nuevo archivo', () => {
      // Arrange
      component.bulkMessage = 'Mensaje previo de éxito';
      component.bulkError = 'Error previo';
      const mockFile = new File(['{}'], 'test.json', { type: 'application/json' });
      const event = { target: { files: [mockFile] } };

      // Act
      component.onBulkFileChange(event);

      // Assert
      expect(component.bulkMessage).toBe('');
      expect(component.bulkError).toBe('');
      expect(component.bulkFile).toBeDefined();
      expect(component.bulkFile?.name).toBe('test.json');
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 3: SUITE 2 - LLAMADA AL SERVICIO
  // --------------------------------------------------------------------------

  describe('SUITE 2: Llamada al Servicio', () => {
    it('Test 2.1: Debe llamar al servicio con el archivo correcto', () => {
      // Arrange
      const mockFile = new File(['{"players": []}'], 'players.json', {
        type: 'application/json',
      });
      component.bulkFile = mockFile;
      mockPlayerService.bulkUpload.and.returnValue(
        of({ message: '✅ Jugadores cargados exitosamente.' })
      );

      // Act
      component.uploadBulk();

      // Assert
      expect(mockPlayerService.bulkUpload).toHaveBeenCalledWith(mockFile);
      expect(mockPlayerService.bulkUpload).toHaveBeenCalledTimes(1);
    });

    it('Test 2.2: No debe llamar al servicio si no hay archivo', () => {
      // Arrange
      component.bulkFile = undefined;

      // Act
      component.uploadBulk();

      // Assert
      expect(mockPlayerService.bulkUpload).not.toHaveBeenCalled();
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 4: SUITE 3 - MANEJO DE RESPUESTAS EXITOSAS
  // --------------------------------------------------------------------------

  describe('SUITE 3: Manejo de Respuestas Exitosas', () => {
    it('Test 3.1: Debe mostrar mensaje de éxito cuando la carga es exitosa', () => {
      // Arrange
      const mockFile = new File(['{}'], 'players.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      const mockResponse = {
        message: '✅ 10 jugadores cargados exitosamente.',
        createdCount: 10,
      };
      mockPlayerService.bulkUpload.and.returnValue(of(mockResponse));

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkMessage).toBe('✅ 10 jugadores cargados exitosamente.');
      expect(component.bulkError).toBe('');
    });

    it('Test 3.2: Debe usar mensaje por defecto si el servidor no envía mensaje', () => {
      // Arrange
      const mockFile = new File(['{}'], 'players.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      const mockResponse = { createdCount: 5 }; // Sin mensaje
      mockPlayerService.bulkUpload.and.returnValue(of(mockResponse));

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkMessage).toBe('✅ Jugadores cargados exitosamente.');
      expect(component.bulkError).toBe('');
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 5: SUITE 4 - MANEJO DE ERRORES DEL BACKEND
  // --------------------------------------------------------------------------

  describe('SUITE 4: Manejo de Errores del Backend', () => {
    it('Test 4.1: Debe mostrar errores específicos del JSON cuando existen', () => {
      // Arrange
      const mockFile = new File(['{}'], 'invalid.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      
      mockPlayerService.bulkUpload.and.returnValue(
        throwError(() => new HttpErrorResponse({
          status: 400,
          error: {
            errors: [
              "Jugador #1 ('Patrick Mahomes'): El nombre es requerido",
              "Jugador #2 ('Tom Brady'): Posición inválida",
              "Jugador #3 ('Aaron Rodgers'): No se encontró la imagen en la ruta especificada",
            ]
          }
        }))
      );

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toBe(
        "Jugador #1 ('Patrick Mahomes'): El nombre es requerido\n" +
          "Jugador #2 ('Tom Brady'): Posición inválida\n" +
          "Jugador #3 ('Aaron Rodgers'): No se encontró la imagen en la ruta especificada"
      );
      expect(component.bulkMessage).toBe('');
    });

    it('Test 4.2: Debe usar ErrorHandlerService cuando no hay errores específicos', () => {
      // Arrange
      const mockFile = new File(['{}'], 'players.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      const errorMessage = 'Error al procesar la carga masiva de jugadores';

      mockPlayerService.bulkUpload.and.returnValue(
        throwError(() => new HttpErrorResponse({ status: 500, statusText: 'Internal Server Error' }))
      );
      mockErrorHandler.handleError.and.returnValue(errorMessage);

      // Act
      component.uploadBulk();

      // Assert
      expect(mockErrorHandler.handleError).toHaveBeenCalledWith(
        jasmine.any(HttpErrorResponse),
        'carga masiva de jugadores'
      );
      expect(component.bulkError).toBe(errorMessage);
      expect(component.bulkMessage).toBe('');
    });

    it('Test 4.3: Debe manejar error de archivo JSON inválido', () => {
      // Arrange
      const mockFile = new File(['invalid json content'], 'invalid.json', {
        type: 'application/json',
      });
      component.bulkFile = mockFile;
      
      mockPlayerService.bulkUpload.and.returnValue(
        throwError(() => new HttpErrorResponse({ 
          status: 400,
          error: { errors: ['El archivo no tiene formato JSON válido.'] }
        }))
      );

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toContain('El archivo no tiene formato JSON válido.');
      expect(component.bulkMessage).toBe('');
    });

    it('Test 4.4: Debe manejar error de archivo vacío', () => {
      // Arrange
      const mockFile = new File(['[]'], 'empty.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      
      mockPlayerService.bulkUpload.and.returnValue(
        throwError(() => new HttpErrorResponse({
          status: 400,
          error: { errors: ['El archivo no contiene datos de jugadores.'] }
        }))
      );

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toContain('El archivo no contiene datos de jugadores.');
      expect(component.bulkMessage).toBe('');
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 6: SUITE 5 - MANEJO DE ERRORES DE RED
  // --------------------------------------------------------------------------

  describe('SUITE 5: Manejo de Errores de Red', () => {
    it('Test 5.1: Debe manejar error de red (sin conexión)', () => {
      // Arrange
      const mockFile = new File(['{}'], 'players.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      const errorMessage = 'Error de conexión con el servidor';

      mockPlayerService.bulkUpload.and.returnValue(
        throwError(() => new HttpErrorResponse({ status: 0, statusText: 'Unknown Error' }))
      );
      mockErrorHandler.handleError.and.returnValue(errorMessage);

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toBe(errorMessage);
      expect(component.bulkMessage).toBe('');
    });

    it('Test 5.2: Debe manejar timeout de servidor', () => {
      // Arrange
      const mockFile = new File(['{}'], 'players.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      const errorMessage = 'Tiempo de espera agotado al procesar la carga masiva';

      mockPlayerService.bulkUpload.and.returnValue(
        throwError(() => new HttpErrorResponse({ status: 504, statusText: 'Gateway Timeout' }))
      );
      mockErrorHandler.handleError.and.returnValue(errorMessage);

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toBe(errorMessage);
      expect(component.bulkMessage).toBe('');
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 7: SUITE 6 - INTEGRACIÓN CON UI
  // --------------------------------------------------------------------------

  describe('SUITE 6: Integración con UI', () => {
    it('Test 6.1: Debe limpiar mensajes al seleccionar nuevo archivo', () => {
      // Arrange
      component.bulkMessage = 'Mensaje de éxito anterior';
      component.bulkError = 'Mensaje de error anterior';
      const mockFile = new File(['{}'], 'new.json', { type: 'application/json' });
      const event = { target: { files: [mockFile] } };

      // Act
      component.onBulkFileChange(event);

      // Assert
      expect(component.bulkMessage).toBe('');
      expect(component.bulkError).toBe('');
    });

    it('Test 6.2: Debe permitir subir otro archivo después de un error', () => {
      // Arrange - Primera carga con error
      const firstFile = new File(['{}'], 'first.json', { type: 'application/json' });
      component.bulkFile = firstFile;
      const errorResponse = {
        error: { errors: ['Error en la primera carga'] },
      };
      mockPlayerService.bulkUpload.and.returnValue(throwError(() => errorResponse));

      // Act - Primera carga
      component.uploadBulk();
      expect(component.bulkError).toBeTruthy();
      expect(component.bulkMessage).toBe('');

      // Arrange - Segunda carga exitosa
      const secondFile = new File(['{}'], 'second.json', { type: 'application/json' });
      const successResponse = { message: '✅ Segunda carga exitosa' };
      mockPlayerService.bulkUpload.and.returnValue(of(successResponse));

      // Act - Cambiar archivo y volver a subir
      component.onBulkFileChange({ target: { files: [secondFile] } });
      component.uploadBulk();

      // Assert
      expect(component.bulkMessage).toBe('✅ Segunda carga exitosa');
      expect(component.bulkError).toBe('');
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 8: PRUEBAS ADICIONALES DE CASOS DE BORDE
  // --------------------------------------------------------------------------

  describe('Casos de Borde Adicionales', () => {
    it('Debe manejar múltiples errores en un solo archivo', () => {
      // Arrange
      const mockFile = new File(['{}'], 'multi-error.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      mockPlayerService.bulkUpload.and.returnValue(
        throwError(
          () =>
            new HttpErrorResponse({
              status: 400,
              error: {
                errors: [
                  'Error en jugador #1',
                  'Error en jugador #2',
                  'Error en jugador #3',
                  'Error en jugador #4',
                  'Error en jugador #5',
                ],
              },
            })
        )
      );

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toContain('Error en jugador #1');
      expect(component.bulkError).toContain('Error en jugador #5');
      const errorLines = component.bulkError.split('\n');
      expect(errorLines.length).toBe(5);
    });

    it('Debe manejar archivo con extensión incorrecta', () => {
      // Arrange
      const mockFile = new File(['{}'], 'players.txt', { type: 'text/plain' });
      component.bulkFile = mockFile;
      mockPlayerService.bulkUpload.and.returnValue(
        throwError(
          () =>
            new HttpErrorResponse({
              status: 400,
              error: { errors: ['El archivo debe ser de tipo JSON'] },
            })
        )
      );

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkError).toContain('El archivo debe ser de tipo JSON');
    });

    it('Debe manejar respuesta exitosa parcial (algunos jugadores creados, otros con error)', () => {
      // Arrange
      const mockFile = new File(['{}'], 'partial.json', { type: 'application/json' });
      component.bulkFile = mockFile;
      const mockResponse = {
        message: '⚠️ 7 de 10 jugadores fueron creados. Revise los errores.',
        createdCount: 7,
        errors: [
          'Jugador #3: Error de validación',
          'Jugador #8: Error de validación',
          'Jugador #10: Error de validación',
        ],
      };
      mockPlayerService.bulkUpload.and.returnValue(of(mockResponse));

      // Act
      component.uploadBulk();

      // Assert
      expect(component.bulkMessage).toBe(
        '⚠️ 7 de 10 jugadores fueron creados. Revise los errores.'
      );
      expect(component.bulkError).toBe('');
    });
  });
});
