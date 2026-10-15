
import { Component, input, output } from '@angular/core';

export interface TableColumn {
  campo: string;
  titulo: string;
  largura?: string;
  alinhamento?: 'left' | 'center' | 'right';
}

export type TableRow = Record<string, unknown>;

@Component({
  selector: 'app-table',
  standalone: true,
  templateUrl: './table.html',
  styleUrl: './table.scss'
})
export class Table {
  readonly colunas = input.required<TableColumn[]>();
  readonly dados = input.required<TableRow[]>();

  readonly carregando = input(false);
  readonly mensagemVazia = input('Nenhum registro encontrado.');

  readonly acaoHabilitada = input(false);
  readonly textoAcao = input('Abrir');

  readonly acao = output<TableRow>();

  executarAcao(registro: TableRow): void {
    this.acao.emit(registro);
  }

  obterValor(registro: TableRow, campo: string): string {
    const valor = registro[campo];

    if (valor === null || valor === undefined || valor === '') {
      return '—';
    }

    return String(valor);
  }
}
