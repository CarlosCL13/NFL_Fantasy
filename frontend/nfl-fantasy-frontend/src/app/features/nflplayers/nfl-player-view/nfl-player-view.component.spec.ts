import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NflPlayerViewComponent } from './nfl-player-view.component';

// Servicios y Router
import { NflPlayerService } from '../../../core/services/nflplayer.service';
import { ErrorHandlerService } from '../../../core/services/error-handler.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';

import { By } from '@angular/platform-browser';

// RxJS
import { of, throwError } from 'rxjs';

describe('NflPlayerViewComponent', () => {
  let component: NflPlayerViewComponent;
  let fixture: ComponentFixture<NflPlayerViewComponent>;

  // 1. Declaramos los espías (Mocks)
  let mockPlayerService: jasmine.SpyObj<NflPlayerService>;
  let mockErrorHandler: jasmine.SpyObj<ErrorHandlerService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    // 2. Creamos los objetos espía con los métodos exactos que usa tu componente
    mockPlayerService = jasmine.createSpyObj('NflPlayerService', [
      'getPlayers',
      'getDesignaciones',
      'getPlayerNews',
      'createPlayerNews',
    ]);
    mockErrorHandler = jasmine.createSpyObj('ErrorHandlerService', ['handleError']);
    mockAuthService = jasmine.createSpyObj('AuthService', ['getUserRole']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    // 3. Configuración inicial de las respuestas (Happy Path por defecto)
    mockPlayerService.getPlayers.and.returnValue(of([{ nflPlayerId: 1, name: 'Tom Brady' }]));
    mockPlayerService.getDesignaciones.and.returnValue(of([]));
    mockPlayerService.getPlayerNews.and.returnValue(of([]));
    mockPlayerService.createPlayerNews.and.returnValue(of({ success: true }));

    // Configuración del módulo de pruebas
    await TestBed.configureTestingModule({
      // Importamos el componente porque es Standalone
      imports: [NflPlayerViewComponent],
      // Proveemos los Mocks en lugar de los servicios reales
      providers: [
        { provide: NflPlayerService, useValue: mockPlayerService },
        { provide: ErrorHandlerService, useValue: mockErrorHandler },
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    // Creación del componente
    fixture = TestBed.createComponent(NflPlayerViewComponent);
    component = fixture.componentInstance;

    // Espiamos window.alert para evitar popups reales durante los tests
    spyOn(window, 'alert');

    // Ejecuta ngOnInit
    fixture.detectChanges();
  });

  // --------------------------------------------------------------------------
  // BLOQUE 1: PRUEBAS BÁSICAS Y DE CICLO DE VIDA
  // --------------------------------------------------------------------------

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar jugadores y designaciones al iniciar (ngOnInit)', () => {
    expect(mockPlayerService.getPlayers).toHaveBeenCalled();
    expect(mockPlayerService.getDesignaciones).toHaveBeenCalled();
    // Validamos que el array de jugadores no esté vacío (basado en el mock)
    expect(component.players.length).toBe(1);
    expect(component.players[0].name).toBe('Tom Brady');
  });

  // --------------------------------------------------------------------------
  // BLOQUE 2: INTERACCIÓN DEL USUARIO
  // --------------------------------------------------------------------------

  it('debe configurar el jugador seleccionado y cargar sus noticias', () => {
    const player = { nflPlayerId: 99, name: 'Mahomes' };

    // Simulamos el clic o la selección
    component.selectPlayer(player);

    expect(component.selectedPlayer).toEqual(player);
    expect(component.showModal).toBeTrue();
    expect(mockPlayerService.getPlayerNews).toHaveBeenCalledWith(99);
  });

  // --------------------------------------------------------------------------
  // BLOQUE 3: LÓGICA DE VALIDACIÓN (validateNews)
  // --------------------------------------------------------------------------

  describe('Validación de Noticias (validateNews)', () => {
    it('debe retornar error si el texto es muy corto', () => {
      component.noticia.texto = 'Hola'; // Menos de 10 caracteres
      const error = component.validateNews();
      expect(error).toContain('entre 10 y 300 caracteres');
    });

    it('debe retornar error si es lesión y falta el resumen', () => {
      component.noticia.texto = 'Texto válido de más de diez caracteres';
      component.noticia.isLesion = true;
      component.noticia.resumen = ''; // Vacío

      const error = component.validateNews();
      expect(error).toContain('resumen es obligatorio');
    });

    it('debe retornar error si es lesión y falta designación', () => {
      component.noticia.texto = 'Texto válido de más de diez caracteres';
      component.noticia.isLesion = true;
      component.noticia.resumen = 'Resumen válido';
      component.noticia.designacionId = null; // Falta ID

      const error = component.validateNews();
      expect(error).toContain('seleccionar una designación');
    });

    it('debe retornar null (válido) si todo está correcto', () => {
      component.noticia.texto = 'Texto correcto para la noticia';
      component.noticia.isLesion = false;

      const error = component.validateNews();
      expect(error).toBeNull();
    });
  });

  // --------------------------------------------------------------------------
  // BLOQUE 4: ENVÍO DE DATOS (SUBMIT)
  // --------------------------------------------------------------------------

  it('debe enviar la noticia si la validación es exitosa', () => {
    // Configuración de datos válidos
    component.selectedPlayer = { nflPlayerId: 1, name: 'Test Player' };
    component.noticia = {
      texto: 'Noticia válida para enviar al servidor',
      resumen: '',
      isLesion: false,
      designacionId: null,
    };

    component.submitNews();

    expect(mockPlayerService.createPlayerNews).toHaveBeenCalled();
    expect(component.submitting).toBeFalse();
    expect(window.alert).toHaveBeenCalledWith('Noticia agregada correctamente.');
  });

  it('no debe enviar la noticia si la validación falla', () => {
    component.noticia.texto = 'Corto'; // Texto inválido

    component.submitNews();

    expect(mockPlayerService.createPlayerNews).not.toHaveBeenCalled();
    expect(component.errorMessage).toBeTruthy();
  });

  // --------------------------------------------------------------------------
  // BLOQUE 5: MANEJO DE ERRORES DEL SERVICIO
  // --------------------------------------------------------------------------

  it('debe manejar el error si el servicio falla al crear noticia', () => {
    // 1. Configuramos el mock para que lance un error simulado
    mockPlayerService.createPlayerNews.and.returnValue(throwError(() => new Error('Error Server')));
    // 2. Configuramos el mock del ErrorHandler para devolver un string
    mockErrorHandler.handleError.and.returnValue('Mensaje amigable de error');

    // 3. Datos válidos para pasar la validación local
    component.selectedPlayer = { nflPlayerId: 1, name: 'Test' };
    component.noticia.texto = 'Texto válido para prueba de error';

    // 4. Ejecutar
    component.submitNews();

    // 5. Verificar
    expect(mockPlayerService.createPlayerNews).toHaveBeenCalled();
    expect(mockErrorHandler.handleError).toHaveBeenCalled();
    expect(component.errorMessage).toBe('Mensaje amigable de error');
    expect(component.submitting).toBeFalse();
  });

  // --------------------------------------------------------------------------
  // BLOQUE 6: TESTS DE INTEGRACIÓN (HTML / DOM) - CORREGIDO
  // --------------------------------------------------------------------------

  it('debe mostrar el mensaje de error en el HTML si hay un error', () => {
    // 1. IMPORTANTE: Abrimos el modal primero, si no el HTML del error no existe
    component.showModal = true;
    component.errorMessage = 'Error visible en pantalla';

    // 2. Actualizamos el HTML
    fixture.detectChanges();

    // 3. Buscamos el elemento exacto basado en tu HTML: <div class="alert alert-danger">
    const errorElement = fixture.debugElement.query(By.css('.alert.alert-danger'));

    // Verificamos que exista y tenga el texto
    expect(errorElement).toBeTruthy('No se encontró el div de error');
    if (errorElement) {
      expect(errorElement.nativeElement.textContent).toContain('Error visible en pantalla');
    }
  });

  it('debe llamar a submitNews() cuando se hace clic en el botón de guardar', () => {
    // 1. Abrimos el modal y seleccionamos jugador
    component.showModal = true;
    component.selectedPlayer = { nflPlayerId: 1, name: 'Test' };
    fixture.detectChanges();

    // 2. Espiamos el método real
    spyOn(component, 'submitNews');

    // 3. Buscamos el botón por su clase exacta: <button class="btn btn-success ...">
    // Usamos el texto para asegurar que es el botón correcto "Guardar noticia"
    const buttons = fixture.debugElement.queryAll(By.css('button.btn-success'));
    const saveButton = buttons.find((btn) => btn.nativeElement.textContent.includes('Guardar'));

    expect(saveButton).toBeTruthy('No se encontró el botón de Guardar noticia');

    if (saveButton) {
      // 4. Simulamos el clic
      saveButton.nativeElement.click();

      // 5. Verificamos que el clic llamó a la función
      expect(component.submitNews).toHaveBeenCalled();
    }
  });

  it('debe limpiar el formulario después de un envío exitoso', () => {
    // 1. Preparamos los datos necesarios (CRÍTICO: selectedPlayer no puede ser null)
    component.selectedPlayer = { nflPlayerId: 1, name: 'Test' };
    component.noticia.texto = 'Texto viejo que debe borrarse';

    // 2. Ejecutamos el envío
    component.submitNews();

    // 3. Verificamos. Según tu lógica, al guardar exitosamente se recarga el modal o se cierra.
    // Si tu lógica limpia el texto:
    expect(component.noticia.texto).toBe('');
  });
});
