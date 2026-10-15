import { Component, effect, inject, input, output, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { FormError } from '../../../shared/components/forms/form-error/form-error';
import { FormErrorService } from '../../../shared/components/forms/services/form-error';
import { ClientesService } from '../clientes.service';
import { Cliente, CriarClienteRequest } from '../cliente.model';
import { map } from 'rxjs';

@Component({
  selector: 'app-cliente-form',
  standalone: true,
  imports: [ReactiveFormsModule, FormError],
  templateUrl: './cliente-form.html',
  styleUrl: './cliente-form.scss',
})
export class ClienteForm {
  private readonly fb = inject(FormBuilder);
  private readonly clientesService = inject(ClientesService);
  private readonly formErrorService = inject(FormErrorService);

  readonly cliente = input<Cliente | null>(null);

  readonly clienteSalvo = output<void>();

  readonly salvando = signal(false);
  readonly erroGeral = signal('');

  readonly formulario = this.fb.nonNullable.group({
    nome: ['', [Validators.required, Validators.minLength(3)]],
    cpfCnpj: [''],
    email: ['', Validators.email],
    telefone: [''],
  });

  constructor() {
    effect(() => {
      const cliente = this.cliente();

      if (cliente) {
        this.formulario.patchValue({
          nome: cliente.nome,
          cpfCnpj: cliente.cpfCnpj ?? '',
          email: cliente.email ?? '',
          telefone: cliente.telefone ?? '',
        });
      } else {
        this.formulario.reset();
      }

      this.erroGeral.set('');
    });
  }

  get modoEdicao(): boolean {
    return this.cliente() !== null;
  }

  get podeSalvar(): boolean {
    return this.formulario.valid && !this.salvando();
  }

  salvar(): void {
    if (!this.podeSalvar) {
      this.formulario.markAllAsTouched();
      return;
    }

    this.salvando.set(true);
    this.erroGeral.set('');

    const valores = this.formulario.getRawValue();

    const dados: CriarClienteRequest = {
      nome: valores.nome.trim(),
      cpfCnpj: valores.cpfCnpj.trim() || null,
      email: valores.email.trim() || null,
      telefone: valores.telefone.trim() || null,
    };

    const cliente = this.cliente();

    const requisicao = cliente
      ? this.clientesService.atualizar(cliente.id, dados)
      : this.clientesService.criar(dados).pipe(map(() => void 0));

    requisicao.subscribe({
      next: () => {
        this.salvando.set(false);
        this.clienteSalvo.emit();
      },
      error: (erro: HttpErrorResponse) => {
        const mensagens = this.formErrorService.aplicarErros(this.formulario, erro);

        if (mensagens.length > 0) {
          this.erroGeral.set(mensagens.join(' '));
        }

        this.salvando.set(false);
      },
    });
  }
}
