
import {
  Component,
  HostListener,
  input,
  output
} from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  templateUrl: './modal.html',
  styleUrl: './modal.scss'
})
export class Modal {
  readonly titulo = input.required<string>();
  readonly descricao = input<string>('');

  readonly fechada = output<void>();

  @HostListener('document:keydown.escape')
  aoPressionarEscape(): void {
    this.fechar();
  }

  fechar(): void {
    this.fechada.emit();
  }

  aoClicarFundo(evento: MouseEvent): void {
    if (evento.target === evento.currentTarget) {
      this.fechar();
    }
  }
}
