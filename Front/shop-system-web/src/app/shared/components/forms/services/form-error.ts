import { Injectable } from '@angular/core';
import { AbstractControl, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Subscription } from 'rxjs';

interface ErroValidacaoApi {
  errors: Record<string, string[]>;
}

@Injectable({
  providedIn: 'root',
})
export class FormErrorService {
  aplicarErros(formulario: FormGroup, resposta: HttpErrorResponse): string[] {
    const dados = resposta.error as Partial<ErroValidacaoApi> | null;

    if (!dados?.errors || typeof dados.errors !== 'object') {
      return ['Não foi possível concluir a operação. Tente novamente.'];
    }

    const errosGerais: string[] = [];

    for (const [campo, mensagens] of Object.entries(dados.errors)) {
      if (!Array.isArray(mensagens)) {
        continue;
      }

      const controle = formulario.get(campo);

      if (!controle) {
        errosGerais.push(...mensagens);
        continue;
      }

      controle.setErrors({
        ...controle.errors,
        server: mensagens.join(' '),
      });

      controle.markAsTouched();
    }

    return errosGerais;
  }

  observarAlteracoes(formulario: FormGroup): Subscription {
    const inscricoes = new Subscription();

    Object.values(formulario.controls).forEach((controle) => {
      const inscricao = controle.valueChanges.subscribe(() => {
        if (!controle.hasError('server')) {
          return;
        }

        const erros = { ...controle.errors };
        delete erros['server'];

        controle.setErrors(Object.keys(erros).length > 0 ? erros : null, { emitEvent: false });
      });

      inscricoes.add(inscricao);
    });

    return inscricoes;
  }
}
