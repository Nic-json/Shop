import { Component, computed, inject, OnInit, signal } from '@angular/core';

import { Cliente } from './cliente.model';
import { ClientesService } from './clientes.service';
import { ClienteForm } from './cliente-form/cliente-form';

import { Tabs, TabItem } from '../../shared/components/tabs/tabs';
import { Modal } from '../../shared/components/modal/modal';
import { Card } from '../../shared/components/card/card';

import { Table, TableColumn, TableRow } from '../../shared/components/table/table';

@Component({
  selector: 'app-clientes',
  standalone: true,
  templateUrl: './clientes.html',
  styleUrl: './clientes.scss',
  imports: [ClienteForm, Tabs, Modal, Card, Table],
})
export class Clientes implements OnInit {
  private readonly clientesService = inject(ClientesService);

  // Lista de clientes
  readonly clientes = signal<Cliente[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal('');

  // Pesquisa de clientes
  readonly busca = signal('');

  // Filtra os clientes conforme o texto pesquisado
  readonly clientesFiltrados = computed(() => {
    const termo = this.busca().trim().toLowerCase();

    if (!termo) {
      return this.clientes();
    }

    return this.clientes().filter((cliente) =>
      [cliente.nome, cliente.cpfCnpj, cliente.email, cliente.telefone].some((valor) =>
        valor?.toLowerCase().includes(termo),
      ),
    );
  });

  // Converte os clientes para o formato da Table
  readonly linhasClientes = computed<TableRow[]>(() =>
    this.clientesFiltrados().map((cliente) => ({
      id: cliente.id,
      nome: cliente.nome,
      cpfCnpj: cliente.cpfCnpj,
      email: cliente.email,
      telefone: cliente.telefone,
    })),
  );

  // Configuração das colunas da tabela
  readonly colunasClientes: TableColumn[] = [
    {
      campo: 'nome',
      titulo: 'Nome',
    },
    {
      campo: 'cpfCnpj',
      titulo: 'CPF/CNPJ',
    },
    {
      campo: 'email',
      titulo: 'E-mail',
    },
    {
      campo: 'telefone',
      titulo: 'Telefone',
    },
  ];

  // Gerenciamento das abas
  readonly abas = signal<TabItem[]>([
    {
      id: 'lista',
      titulo: 'Clientes',
      fechavel: false,
    },
  ]);

  readonly abaAtiva = signal('lista');

  // Controle da Modal de cadastro
  readonly modalClienteAberta = signal(false);
  readonly clienteEmEdicao = signal<Cliente | null>(null);

  ngOnInit(): void {
    this.carregarClientes();
  }

  // Busca os clientes cadastrados na API
  carregarClientes(): void {
    this.carregando.set(true);
    this.erro.set('');

    this.clientesService.listar().subscribe({
      next: (clientes) => {
        this.clientes.set(clientes);
        this.carregando.set(false);
      },
      error: (erro) => {
        console.error('Erro ao buscar clientes:', erro);

        this.erro.set('Não foi possível carregar os clientes.');

        this.carregando.set(false);
      },
    });
  }

  // Atualiza o texto da pesquisa
  atualizarBusca(evento: Event): void {
    const input = evento.target as HTMLInputElement;
    this.busca.set(input.value);
  }

  // Recebe a ação emitida pelo componente Table
  abrirClienteDaTabela(registro: TableRow): void {
    const id = registro['id'];

    if (typeof id !== 'string') {
      return;
    }

    const cliente = this.clientes().find((cliente) => cliente.id === id);

    if (cliente) {
      this.abrirCliente(cliente);
    }
  }

  // Abre uma aba individual para o cliente
  abrirCliente(cliente: Cliente): void {
    const idAba = `cliente-${cliente.id}`;

    const jaExiste = this.abas().some((aba) => aba.id === idAba);

    if (!jaExiste) {
      this.abas.update((abas) => [
        ...abas,
        {
          id: idAba,
          titulo: cliente.nome,
          fechavel: true,
        },
      ]);
    }

    this.abaAtiva.set(idAba);
  }

  // Alterna entre as abas
  selecionarAba(id: string): void {
    this.abaAtiva.set(id);
  }

  // Fecha uma aba individual
  fecharAba(id: string): void {
    this.abas.update((abas) => abas.filter((aba) => aba.id !== id));

    if (this.abaAtiva() === id) {
      this.abaAtiva.set('lista');
    }
  }

  // Abre a Modal de cadastro
  abrirModalNovoCliente(): void {
    this.clienteEmEdicao.set(null);
    this.modalClienteAberta.set(true);
  }

  abrirModalEditarCliente(cliente: Cliente): void {
    this.clienteEmEdicao.set(cliente);
    this.modalClienteAberta.set(true);
  }

  // Fecha a Modal de cadastro
  fecharModalCliente(): void {
    this.modalClienteAberta.set(false);
    this.clienteEmEdicao.set(null);
  }

  // Executado após cadastrar ou editar um cliente
  aoSalvarCliente(): void {
    const clienteEditado = this.clienteEmEdicao();

    if (clienteEditado) {
      // Atualiza o título da aba quando o nome for alterado
      // após recebermos os dados atualizados da API.
      this.clientesService.listar().subscribe({
        next: (clientes) => {
          this.clientes.set(clientes);

          const atualizado = clientes.find((cliente) => cliente.id === clienteEditado.id);

          if (atualizado) {
            this.abas.update((abas) =>
              abas.map((aba) =>
                aba.id === `cliente-${atualizado.id}` ? { ...aba, titulo: atualizado.nome } : aba,
              ),
            );
          }
        },
        error: (erro) => {
          console.error('Erro ao atualizar lista de clientes:', erro);
          this.erro.set('Não foi possível atualizar a lista de clientes.');
        },
      });
    } else {
      this.carregarClientes();
    }

    this.fecharModalCliente();
  }
}
