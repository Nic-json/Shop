
import { Component, input } from '@angular/core';

@Component({
  selector: 'app-card',
  standalone: true,
  templateUrl: './card.html',
  styleUrl: './card.scss'
})
export class Card {
  readonly titulo = input<string>('');
  readonly descricao = input<string>('');
  readonly semPadding = input<boolean>(false);
}
