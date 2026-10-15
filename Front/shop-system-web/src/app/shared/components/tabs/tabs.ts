
import { Component, input, output } from '@angular/core';

export interface TabItem {
  id: string;
  titulo: string;
  fechavel?: boolean;
}

@Component({
  selector: 'app-tabs',
  standalone: true,
  templateUrl: './tabs.html',
  styleUrl: './tabs.scss'
})
export class Tabs {
  readonly abas = input.required<TabItem[]>();
  readonly abaAtiva = input.required<string>();

  readonly abaSelecionada = output<string>();
  readonly abaFechada = output<string>();

  selecionar(id: string): void {
    if (this.abaAtiva() === id) return;

    this.abaSelecionada.emit(id);
  }

  fechar(id: string): void {
    this.abaFechada.emit(id);
  }
}
