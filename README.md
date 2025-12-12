# 📦 Sistema de Gestão de Estoque

Aplicação Fullstack para gerenciamento de estoque, desenvolvida com **Angular** no frontend e **.NET (C#)** no backend.

## 🚀 Tecnologias

### Frontend
* **Angular**
* **TypeScript**
* **HTML/SCSS**
* **Angular Material** (ou Bootstrap, se aplicável)

### Backend
* **C# / .NET Core** (Web API)
* **Entity Framework Core**
* **SQL Server** (ou banco de dados utilizado)

---

## 📋 Pré-requisitos

Para rodar este projeto, você precisará ter instalado:

1.  **Node.js** (Versão LTS): [Download](https://nodejs.org/)
2.  **Angular CLI**:
    ```bash
    npm install -g @angular/cli
    ```
3.  **.NET SDK** (Compatível com a versão do projeto, ex: .NET 6, 7 ou 8): [Download](https://dotnet.microsoft.com/download)
4.  **SQL Server** (Caso o backend utilize banco local).

---

## 🔧 Como Rodar o Projeto

É necessário rodar o Backend e o Frontend simultaneamente em terminais separados.

### 1️⃣ Configurando o Backend (API)

1.  Abra um terminal e navegue até a pasta do servidor:
    ```bash
    cd backend
    ```
2.  Restaure as dependências do projeto:
    ```bash
    dotnet restore
    ```
3.  Configure o Banco de Dados (se necessário):
    * Verifique o arquivo `appsettings.json` para confirmar a **ConnectionString**.
    * Se houver migrações pendentes, execute: `dotnet ef database update`
4.  Inicie a API:
    ```bash
    dotnet run
    ```
    > O backend deve iniciar (geralmente em `http://localhost:5000` ou `https://localhost:5001`). Verifique o terminal para ver a URL correta.

### 2️⃣ Configurando o Frontend (Angular)

1.  Abra um **novo terminal** e navegue até a pasta do cliente:
    ```bash
    cd frontend
    ```
2.  Instale as dependências do Angular:
    ```bash
    npm install
    ```
3.  Inicie o servidor de desenvolvimento:
    ```bash
    ng serve
    ```
4.  Acesse a aplicação no navegador:
    * **URL:** `http://localhost:4200/`

---

## ⚙️ Configurações Importantes

* **API URL:** Caso a API não esteja rodando na porta padrão, vá até `frontend/src/environments/environment.ts` e ajuste a variável `apiUrl` para corresponder ao endereço do seu Backend.
* **CORS:** Se houver erros de bloqueio (CORS) no navegador, verifique no arquivo `Program.cs` (ou `Startup.cs`) do Backend se a origem `http://localhost:4200` está permitida.

## 🤝 Contribuição

1.  Faça um Fork do projeto
2.  Crie sua Feature Branch (`git checkout -b feature/MinhaFeature`)
3.  Commit suas mudanças (`git commit -m 'Adicionando nova feature'`)
4.  Push para a Branch (`git push origin feature/MinhaFeature`)
5.  Abra um Pull Request
