# 📇 Agenda de Contatos

Sistema completo de gerenciamento de contatos com CRUD, busca e paginação server-side.

---

## 🚀 Tecnologias Utilizadas

### 🔹 Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Repository Pattern
- DTO Pattern
- Paginação server-side

### 🔹 Frontend
- Vue 3 (Composition API)
- TypeScript
- Vuetify 3
- Pinia
- Axios
- Maska (máscara de telefone)

---

## ✅ Pré-requisitos

### Rodando com Docker (recomendado)
- Docker Desktop instalado e rodando
- Docker Compose (já vem no Docker Desktop)

### Rodando local (sem Docker)
- .NET SDK 8
- Node.js 18+ (ou 20+)
- PostgreSQL instalado localmente (opcional se usar Docker)

---

# 🐳 Como rodar o projeto com Docker + Docker Compose
> Este modo sobe automaticamente o **PostgreSQL + API (.NET) + Front (Vue) + Nginx** com um comando.

## 1- Ir para a pasta `infra`
No Windows PowerShell:

```bash
cd infra
```
## 2- Criar arquivo .env (na pasta infra)
Crie um arquivo chamado .env dentro de infra/ com:

```bash
POSTGRES_DB=agenda_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_PORT=5432
```

## 3- Subir os containers
```bash
docker compose up --build
```

## 4- Acesso padrão 
```
Frontend (Nginx): http://localhost:80

API: http://localhost:8080

Postgres: localhost:5432

Se no seu compose as portas forem outras, ajuste conforme seu docker-compose.yml.
```

### 5- Parar os containers
```bash
docker compose down
```

### 5- Parar e apagar volumes (zerar o banco)
```bash
docker compose down -v
```

# 🧪 Como rodar sem Docker (modo dev)
## Backend (.NET)
## 1- Ir para a pasta `backend`
No Windows PowerShell:

```bash
cd backend
```

## Configure a connection string (exemplo no appsettings.Development.json do Agenda.Api):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=agenda_db;Username=postgres;Password=postgres"
  }
}
```

---

## 🗄️ Migrations e Banco de Dados

O projeto utiliza o **Entity Framework Core** (Approach Code-First). Para gerenciar o esquema do banco de dados localmente, siga os comandos abaixo:

### 1. Requisito
Certifique-se de ter a ferramenta `dotnet-ef` instalada:
```bash
dotnet tool install --global dotnet-ef
```
### 2. Aplicar Migrations
Para criar as tabelas no seu banco de dados local (conforme a connection string configurada):

```bash
dotnet ef database update --project Agenda.Api
```

[!NOTE]
Se você estiver utilizando o Docker Compose, as migrations são executadas automaticamente pelo container da API durante o startup, não sendo necessário rodar os comandos acima manualmente.

## Rode a Api

```bash
dotnet run --project Agenda.Api
```
API disponível em:
> `https://localhost:7045` (ou a porta exibida no console)

# Frontend (Vue)
## 1- Ir para a pasta `frontend`
No Windows PowerShell:

```bash
cd frontend
```
## 2- Intale as dependencias

```bash
npm install
```

## 3- Rode o frontend

```bash
npm run dev
```
Frontend disponível em:
> `http://localhost:5173`

# 📞 Contacts API

API para gerenciamento de contatos com suporte a busca fonética, paginação e operações CRUD completas.

---

## 🚀 Base URL
A API está disponível no endereço:
> `https://localhost:7045`

---

## 📑 Endpoints

### 1. Listar Contatos (com busca e paginação)
Retorna uma lista de contatos que podem ser filtrados por termos de busca.

* **URL:** `/Contacts`
* **Método:** `GET`
* **Query Params:**

| Parâmetro | Tipo | Obrigatório | Descrição |
| :--- | :--- | :--- | :--- |
| `search` | string | Não | Busca por nome, email ou telefone |
| `page` | int | Não | Número da página (padrão: 1) |
| `pageSize` | int | Não | Itens por página (padrão: 10) |

#### 🔹 Exemplo de Requisição
`GET /Contacts?search=luan&page=1&pageSize=10`

#### 🔹 Resposta (200 OK)
```json
{
  "items": [
    {
      "id": "6c1fbb4a-3c41-4e65-9d78-4c54a9b75f63",
      "name": "Luan Henrique",
      "email": "luan@email.com",
      "phone": "81999999999",
      "createdAt": "2026-02-25T02:13:02.554248Z",
      "updatedAt": null
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalItems": 1,
  "totalPages": 1
}
```

### 2. Buscar Contato por ID (com busca e paginação)
Retorna os detalhes de um único contato através do seu identificador único (GUID).

* **URL:** `/Contacts/{id}`
* **Método:** `GET`

#### 🔹 Resposta (200 OK)
```json
{
    "id": "6c1fbb4a-3c41-4e65-9d78-4c54a9b75f63",
    "name": "Luan Henrique",
    "email": "luan@email.com",
    "phone": "81999999999",
    "createdAt": "2026-02-25T02:13:02.554248Z",
    "updatedAt": null
}

```

### 3. Criar Contato
Cria um novo registro de contato no banco de dados.

* **URL:** `/Contacts`
* **Método:** `POST`
* **Request Body:**

```json
{
  "name": "João Silva",
  "email": "joao@email.com",
  "phone": "81988888888"
}
```

#### 🔹 Resposta 201 (Created)
```json
{
  "id": "f2c5a89c-1e8c-4f5a-9d6f-3b8a9c44e2f2",
  "name": "João Silva",
  "email": "joao@email.com",
  "phone": "81988888888",
  "createdAt": "2026-02-25T02:20:00.000000Z",
  "updatedAt": null
}

```

### 4. Atualizar Contato
Altera as informações de um contato existente.

* **URL:** `/Contacts/{id}`
* **Método:** `PUT`
* **Request Body:**

```json
{
  "name": "João Silva Atualizado",
  "email": "joao@email.com",
  "phone": "81988888888"
}
```
#### 🔹 Resposta (200 OK)
```json
{
  "id": "f30aba7f-3619-49a4-a2f0-55b7771207b7",
  "name": "João Silva Atualizado",
  "email": "joao@email.com",
  "phone": "81988888888",
  "createdAt": "2026-02-25T01:00:05.7819705Z",
  "updatedAt": "2026-03-25T01:00:05.7819705Z"
}
```

### 5. Deletar Contato
Remove um contato definitivamente.

* **URL:** `/Contacts/{id}`
* **Método:** `DELETE`

#### 🔹 Resposta (204 No Content)


Desenvolvido por Luan Henrique 🚀