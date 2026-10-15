import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-store-identity',
  standalone: true,
  templateUrl: './store-identity.html',
  styleUrl: './store-identity.scss',
})
export class StoreIdentity {
  @Input() nomeEmpresa = 'Minha Empresa';
  @Input() nomeLoja = 'Unidade Principal';
  @Input() logoUrl: string | null = null;

  logoComErro = false;

  get iniciais(): string {
    return this.nomeEmpresa
      .trim()
      .split(/\s+/)
      .slice(0, 2)
      .map(palavra => palavra.charAt(0))
      .join('')
      .toUpperCase();
  }
}