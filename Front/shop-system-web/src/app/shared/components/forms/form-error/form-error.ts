import { Component, Input, signal, OnInit, OnDestroy } from '@angular/core';

import { AbstractControl } from '@angular/forms';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-form-error',
  standalone: true,
  templateUrl: './form-error.html',
  styleUrl: './form-error.scss',
})
export class FormError implements OnInit, OnDestroy {
  @Input({ required: true })
  control!: AbstractControl;

  @Input()
  mensagens: Record<string, string> = {};

  private inscricao?: Subscription;

  private readonly versao = signal(0);

  ngOnInit(): void {
    this.inscricao = this.control.events.subscribe(() => {
      this.versao.update((valor) => valor + 1);
    });
  }

  ngOnDestroy(): void {
    this.inscricao?.unsubscribe();
  }

  get mensagemErro(): string | null {
    // Faz o Angular acompanhar as mudanças do controle.
    this.versao();

    if (!this.control?.invalid || !this.control.touched) {
      return null;
    }

    const erros = this.control.errors;

    if (!erros) {
      return null;
    }

    if (erros['server']) {
      return String(erros['server']);
    }

    for (const tipoErro of Object.keys(erros)) {
      if (this.mensagens[tipoErro]) {
        return this.mensagens[tipoErro];
      }
    }

    return null;
  }
}
