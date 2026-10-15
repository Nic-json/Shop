
export interface Cliente {
  id: string;
  nome: string;
  cpfCnpj: string | null;
  email: string | null;
  telefone: string | null;
}

export interface CriarClienteRequest {
  nome: string;
  cpfCnpj: string | null;
  email: string | null;
  telefone: string | null;
}

export interface CriarClienteResponse {
  id: string;
  mensagem: string;
}
