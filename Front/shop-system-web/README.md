# ShopSys — interface web

Sistema de gestão comercial em desenvolvimento, com interface Angular e API ASP.NET Core. O fluxo principal implementado na interface é o gerenciamento de clientes.

## Estado atual

- Listagem e busca local por nome, CPF/CNPJ, e-mail ou telefone.
- Cadastro e edição de clientes em modal, integrados à API.
- Consulta de dados cadastrais em abas individuais.
- Validação de formulário e exibição de erros retornados pela API.
- Estados de carregamento, lista vazia e falha de consulta com opção de tentar novamente.
- Componentes compartilhados de tabela, abas, modal, card e mensagens de validação.
- Navegação com carregamento de páginas sob demanda e dashboard inicial.

A API também possui endpoints de empresas e lojas e desativação de clientes, ainda sem telas correspondentes. Produtos possuem uma estrutura inicial de entidade e migration.

## Tecnologias

| Camada | Tecnologias |
| --- | --- |
| Interface | Angular 22.2, TypeScript 6, SCSS, signals, Reactive Forms e RxJS |
| API | ASP.NET Core (.NET 10), Entity Framework Core e Npgsql |
| Banco | PostgreSQL 17, com Docker Compose para desenvolvimento |
| Testes da interface | Vitest, pelo executor do Angular |

## Estrutura

Caminhos relativos à raiz do repositório:

```text
Shop_System/
├── Back/Src/Shop.API/
│   ├── Compartilhado/        # Dados, validadores e tratamento de erros
│   ├── Modulos/             # Clientes, empresas, lojas e produtos
│   └── Migrations/          # Evolução do esquema do banco
├── Front/shop-system-web/
│   └── src/app/
│       ├── layout/          # Shell, menu lateral e cabeçalho
│       ├── pages/           # Dashboard e clientes
│       └── shared/          # Componentes reutilizáveis
└── docker-compose.yml
```

## Executar localmente

Pré-requisitos: Node.js compatível com Angular 22.2, npm (o projeto declara a versão 11.19.0), SDK .NET 10 e Docker com Compose. Para aplicar migrations, é necessário o CLI `dotnet-ef` da versão principal 10.

Os exemplos usam PowerShell. Comece cada etapa na raiz do repositório, em terminais separados.

### 1. Iniciar o banco

Defina uma senha local e inicie o serviço:

```powershell
$env:POSTGRES_PASSWORD = 'substitua-por-uma-senha-local'
docker compose up -d postgres
```

O Compose cria o banco `shopsys`, com usuário `shopsys`, na porta `5432`. Os dados ficam no volume `shopsys_postgres_data`. Aguarde o PostgreSQL estar pronto antes de aplicar as migrations.

### 2. Preparar e iniciar a API

Em outro terminal, configure a conexão com a mesma senha usada no banco:

```powershell
cd Back/Src/Shop.API
$env:ConnectionStrings__DefaultConnection = 'Host=localhost;Port=5432;Database=shopsys;Username=shopsys;Password=substitua-por-uma-senha-local'
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

Caso ainda não tenha o CLI do Entity Framework instalado, instale antes de aplicar as migrations:

```powershell
dotnet tool install --global dotnet-ef --version '10.*'
```

A API fica em `http://localhost:5225`. Em desenvolvimento, o documento OpenAPI fica em `http://localhost:5225/openapi/v1.json`.

As variáveis de ambiente dos exemplos valem para o terminal em que foram definidas. O arquivo `.env` do Docker Compose não configura automaticamente a conexão da API.

### 3. Iniciar a interface

Em outro terminal:

```powershell
cd Front/shop-system-web
npm ci
npm start
```

Acesse `http://localhost:4200` ou diretamente `http://localhost:4200/clientes`.

Se o PowerShell bloquear o script `npm.ps1`, use `npm.cmd` no lugar de `npm` nos comandos.

## Comandos da interface

Execute dentro de `Front/shop-system-web`:

| Comando | Finalidade |
| --- | --- |
| `npm start` | Iniciar o servidor de desenvolvimento |
| `npm run build` | Gerar a compilação de produção em `dist/shop-system-web/` |
| `npm run watch` | Recompilar em desenvolvimento a cada alteração |
| `npm test` | Executar os testes configurados com Vitest |
| `npm test -- --watch=false` | Executar os testes sem modo de observação |

Não há executor de testes de ponta a ponta configurado no projeto.

## Integração e limitações atuais

- A URL da API está definida em `src/app/pages/clientes/clientes.service.ts` como `http://localhost:5225/api/clientes`.
- O CORS da API permite a origem `http://localhost:4200`. Mudanças de porta ou domínio devem ser refletidas na configuração.
- A pesquisa de clientes usa a lista carregada em memória, sem paginação no servidor.
- Clientes e lojas usam uma empresa fixa para desenvolvimento: `11111111-1111-1111-1111-111111111111`. A identificação da empresa por usuário autenticado ainda precisa ser implementada.
- A API ainda não possui autenticação e autorização configuradas.
- As validações de cadastro e edição de clientes ainda precisam ser uniformizadas.

O projeto está em construção; essas funcionalidades devem ser consolidadas antes do uso em produção.
