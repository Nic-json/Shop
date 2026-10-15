
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { of } from 'rxjs';
import { vi } from 'vitest';

import { ClienteForm } from './cliente-form';
import { ClientesService } from '../clientes.service';
import { FormErrorService } from '../../../shared/components/forms/services/form-error';

describe('ClienteForm', () => {
  let fixture: ComponentFixture<ClienteForm>;
  let component: ClienteForm;

  const clientesServiceMock = {
    criar: vi.fn(),
    atualizar: vi.fn(),
  };

  beforeEach(async () => {
    vi.clearAllMocks();

    clientesServiceMock.criar.mockReturnValue(of({}));
    clientesServiceMock.atualizar.mockReturnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [ClienteForm],
      providers: [
        provideHttpClient(),
        {
          provide: ClientesService,
          useValue: clientesServiceMock,
        },
        FormErrorService,
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ClienteForm);
    component = fixture.componentInstance;

    await fixture.whenStable();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('não deve permitir salvar com formulário vazio', () => {
    expect(component.formulario.invalid).toBe(true);
    expect(component.podeSalvar).toBe(false);

    component.salvar();

    expect(clientesServiceMock.criar).not.toHaveBeenCalled();
  });

  it('não deve permitir nome com menos de 3 caracteres', () => {
    component.formulario.patchValue({
      nome: 'Ni',
    });

    expect(component.formulario.controls.nome.invalid).toBe(true);
    expect(component.podeSalvar).toBe(false);
  });

  it('deve permitir salvar quando o nome for válido', () => {
    component.formulario.patchValue({
      nome: 'Nicolas',
    });

    expect(component.formulario.valid).toBe(true);
    expect(component.podeSalvar).toBe(true);
  });

  it('deve chamar o serviço e emitir evento ao cadastrar', () => {
    const emitirSpy = vi.spyOn(component.clienteSalvo, 'emit');

    component.formulario.patchValue({
      nome: 'Nicolas',
      cpfCnpj: '',
      email: 'nicolas@teste.com',
      telefone: '',
    });

    component.salvar();

    expect(clientesServiceMock.criar).toHaveBeenCalledWith({
      nome: 'Nicolas',
      cpfCnpj: null,
      email: 'nicolas@teste.com',
      telefone: null,
    });

    expect(emitirSpy).toHaveBeenCalledOnce();
    expect(component.salvando()).toBe(false);
  });

  it('deve preencher o formulário ao editar um cliente', async () => {
    const cliente = {
      id: '11111111-1111-1111-1111-111111111111',
      nome: 'Nicolas Muller',
      cpfCnpj: '52998224725',
      email: 'nicolas@teste.com',
      telefone: '51999999999',
    };

    fixture.componentRef.setInput('cliente', cliente);

    await fixture.whenStable();

    expect(component.modoEdicao).toBe(true);

    expect(component.formulario.getRawValue()).toEqual({
      nome: 'Nicolas Muller',
      cpfCnpj: '52998224725',
      email: 'nicolas@teste.com',
      telefone: '51999999999',
    });
  });

  it('deve atualizar um cliente existente sem criar outro', async () => {
    const cliente = {
      id: '11111111-1111-1111-1111-111111111111',
      nome: 'Nicolas Muller',
      cpfCnpj: null,
      email: 'nicolas@teste.com',
      telefone: null,
    };

    fixture.componentRef.setInput('cliente', cliente);

    await fixture.whenStable();

    const emitirSpy = vi.spyOn(component.clienteSalvo, 'emit');

    component.formulario.patchValue({
      nome: 'Nicolas Atualizado',
    });

    component.salvar();

    expect(clientesServiceMock.atualizar).toHaveBeenCalledWith(
      cliente.id,
      {
        nome: 'Nicolas Atualizado',
        cpfCnpj: null,
        email: 'nicolas@teste.com',
        telefone: null,
      }
    );

    expect(clientesServiceMock.criar).not.toHaveBeenCalled();

    expect(emitirSpy).toHaveBeenCalledOnce();
    expect(component.salvando()).toBe(false);
  });
});
