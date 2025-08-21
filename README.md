# LanguageCourses API

## API RESTful desenvolvida em .NET 8 para gerenciar cursos de idiomas e suas turmas.
## O sistema foi estruturado com MVC, DDD (Domain-Driven Design) e CQRS (Command Query Responsibility Segregation), utilizando Entity Framework Core com Code First para criação e versionamento do banco de dados via Migrations.

---

## Sumário
- [Descrição](#descrição)
- [Funcionalidades](#funcionalidades)
- [Regras de Negócio](#regras-de-negócio)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Instalação](#instalação)
- [Execução](#execução)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Endpoints Principais](#endpoints-principais)
- [Fluxo de Dados](#fluxo-de-dados)

---

## 📖 Descrição
O **LanguageCourses** é uma API para gerenciamento de alunos e turmas em cursos de idiomas.  
O projeto segue boas práticas de **Clean Architecture** e foca em **escalabilidade** e **validações robustas**.

---

### Padrões Utilizados

- MVC (Model-View-Controller) → API organizada com Controllers, Models (ViewModels) e Services.

- DDD (Domain-Driven Design) → Entidades ricas (Aluno, Turma, AlunoTurma) encapsulam regras de negócio.

- CQRS (Command Query Responsibility Segregation) → Separação entre:

- Commands (escrita/mutação de dados) com MediatR.

- Queries (leitura de dados) otimizadas para consulta.

- FluentValidation → Validações de CPF, email, duplicação de turmas, vagas, etc.

 - Entity Framework Core → ORM com Code First, migrations e SQL Server.

 - Swagger/OpenAPI → Documentação interativa da API.

---

## CQRS

**CQRS (Command Query Responsibility Segregation)** é um padrão arquitetural que separa as operações de **escrita** (commands) das operações de **leitura** (queries).  
Com isso, o código fica mais organizado, facilita validações de regras de negócio e permite otimizar consultas e comandos de forma independente.  

- **Command**: altera o estado do sistema (ex.: criar, atualizar, excluir).  
- **Query**: consulta informações sem modificar o estado.  


---

## Funcionalidades
- Cadastro, atualização e exclusão de **Alunos**
- Cadastro, atualização e exclusão de **Turmas**
- Busca de aluno por **ID** e **CPF**
- Relacionamento **Aluno ↔ Turma**
- Validações automáticas de dados (CPF, email, número de turmas, etc.)

---

## Regras de Negócio
- O **Aluno** deve ter no mínimo **1 turma** associada.
- Um **Aluno** não pode estar em **turmas duplicadas**.
- Cada **Turma** pode ter **no máximo 5 alunos**.
- O **CPF** e o **email** devem ser **válidos e únicos**.
- O campo **AnoLetivo** aceita valores no formato `2025/2`.

---

## Tecnologias Utilizadas
- [.NET 8](https://dotnet.microsoft.com/)  
- [Entity Framework Core](https://learn.microsoft.com/ef/core/)  
- [CQRS + MediatR](https://github.com/jbogard/MediatR)  
- [FluentValidation](https://fluentvalidation.net/)  
- [SQL Server] (ajustar conforme banco que está usando)  

---

##  Instalação
1. Clonar o repositório:
   ```bash
   git clone https://github.com/Eduardo-Ruan-G-Fonseca/language-courses.git
   cd language-courses

   ```
2. Restaurar dependências:
   ```bash
   dotnet restore
   ```
3. Criar o banco de dados (ajuste conforme provider):
   ```bash
   dotnet ef database update
   ```

---

##  Execução

- Rodar o projeto via CLI:

```bash
   dotnet run --project src/LanguageCourses.API
```

---

##  Estrutura do Projeto
```
src/
 ├── LanguageCourses.API/           # Camada de apresentação (Controllers, Swagger)
     └─Controller/
 ├── LanguageCourses.Application/   # Casos de uso (Commands, Queries, Handlers, Validators, DTOs)
     └─Commands
     └─Queries
     └─Validators
     └─ViewModels
 ├── LanguageCourses.Domain/        # Entidades e regras de negócio (Aluno, Turma, AlunoTurma)
     └─Entities
 ├── LanguageCourses.Infrastructure/# Acesso a dados (DbContext, Repositories, Migrations)
     └─EF
        └─Configurations
     └─Migrations
```
---

##  Endpoints Principais

 **Aluno**

<img width="1634" height="543" alt="image" src="https://github.com/user-attachments/assets/5e266e32-02ba-4a53-9c98-cbbd9124004b" />

**Turma**

<img width="1625" height="505" alt="image" src="https://github.com/user-attachments/assets/97d903a1-ff28-4b5b-a3f2-2adf8004c36d" />

---

###  Fluxo de Dados (CQRS + MediatR)

1. O **usuário** faz uma requisição para a API.  
2. A requisição chega no **Controller**, que não contém regra de negócio.  
3. O Controller envia a requisição para o **MediatR**, que atua como um mediador.  
4. O MediatR direciona:
   - **Commands** → para **Command Handlers**, que escrevem no banco de dados.  
   - **Queries** → para **Query Handlers**, que apenas leem dados do banco.  
5. O **banco de dados** responde.  
6. O resultado volta pelo mesmo caminho até o **usuário**.

- O usuário faz uma requisição.  
- A API passa para o **MediatR**.  
- O MediatR escolhe se vai para um **Command Handler** (escrita) ou um **Query Handler** (leitura).  
- O handler acessa o **banco de dados** e devolve o resultado para o usuário.  
---
