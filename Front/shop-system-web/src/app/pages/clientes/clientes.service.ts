
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Cliente,
  CriarClienteRequest,
  CriarClienteResponse
} from './cliente.model';

@Injectable({
  providedIn: 'root'
})

export class ClientesService {
  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:5225/api/clientes';

  listar(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.apiUrl);
  }

  criar(dados: CriarClienteRequest): Observable<CriarClienteResponse> {
    return this.http.post<CriarClienteResponse>(
      this.apiUrl,
      dados
    );
  }
  
  atualizar(id: string, dados: CriarClienteRequest): Observable<void> {
    return this.http.put<void>(
      `${this.apiUrl}/${id}`,
      dados
    );
  }
}
