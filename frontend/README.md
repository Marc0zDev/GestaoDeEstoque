# Sistema de Gestão de Estoque - Frontend

Este é o frontend do Sistema de Gestão de Estoque desenvolvido com Angular, PrimeNG e PrimeFlex.

## Tecnologias Utilizadas

- **Angular 18+** - Framework de desenvolvimento
- **PrimeNG** - Biblioteca de componentes UI
- **PrimeFlex** - Utilitários CSS
- **PrimeIcons** - Biblioteca de ícones
- **TypeScript** - Linguagem de programação
- **SCSS** - Pré-processador CSS

## Estrutura do Projeto

```
frontend/
└── gestao-estoque-app/
    ├── src/
    │   ├── app/
    │   │   ├── app.config.ts      # Configuração da aplicação
    │   │   ├── app.ts             # Componente principal
    │   │   ├── app.html           # Template principal
    │   │   ├── app.scss           # Estilos do componente
    │   │   └── app.routes.ts      # Configuração de rotas
    │   ├── styles.scss            # Estilos globais
    │   └── main.ts                # Ponto de entrada da aplicação
    ├── package.json               # Dependências do projeto
    └── angular.json               # Configuração do Angular CLI
```

## Comandos Disponíveis

### Desenvolvimento
```bash
npm start
# ou
ng serve
```
Inicia o servidor de desenvolvimento em http://localhost:4200

### Build de Produção
```bash
npm run build
# ou
ng build
```
Gera os arquivos otimizados para produção na pasta `dist/`

### Testes
```bash
npm test
# ou
ng test
```
Executa os testes unitários

### Linting
```bash
npm run lint
# ou
ng lint
```
Executa a verificação de código

## Configuração do PrimeNG

O projeto está configurado para usar o PrimeNG v18+ com o novo sistema de temas. A configuração é feita em `app.config.ts`:

```typescript
import { providePrimeNG } from 'primeng/config';
import Lara from '@primeuix/themes/lara';

export const appConfig: ApplicationConfig = {
  providers: [
    // ... outros providers
    providePrimeNG({
      theme: {
        preset: Lara
      }
    })
  ]
};
```

## Estilos Globais

Os estilos globais estão configurados em `src/styles.scss`:

```scss
/* PrimeFlex - Utilitários CSS */
@import "primeflex/primeflex.css";

/* PrimeIcons - Ícones */
@import "primeicons/primeicons.css";
```

## Próximos Passos

1. **Criar componentes específicos** para funcionalidades do sistema
2. **Implementar roteamento** para diferentes páginas
3. **Configurar serviços** para comunicação com o backend
4. **Adicionar formulários reativas** para entrada de dados
5. **Implementar autenticação** e controle de acesso
6. **Adicionar tabelas e listagens** para exibição de dados
7. **Configurar interceptadores** para requisições HTTP

## Desenvolvedor

Projeto criado com Angular CLI versão 18.3.8