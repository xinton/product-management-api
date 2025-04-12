# Product Management API

Esta API implementa um sistema de gerenciamento de produtos utilizando .NET 8 com Minimal APIs, Entity Framework Core com abordagem Code First e SQL Server em Docker.

## Estrutura do Projeto

```
apiB2e/
├── Endpoints/
│   ├── AuthenticationEndpoints.cs
│   └── ProductEndpoints.cs
├── Entities/
│   ├── Product.cs
│   └── User.cs
├── Infrastructure/
│   ├── AppDbContext.cs
│   └── Repositories/
│       ├── ProductRepository.cs
│       └── UserRepository.cs
├── Interfaces/
│   ├── IProductRepository.cs
│   └── IUserRepository.cs
├── Services/
│   └── ExcelExportService.cs
└── Program.cs
```

## Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose
- Git

## Como executar o projeto

### Usando Docker Compose (recomendado)

1. Clone o repositório:
   ```bash
   git clone https://github.com/xinton/product-management-api
   cd apiB2e
   ```

2. Execute o Docker Compose:
   ```bash
   docker-compose up --build
   ```

3. A API estará disponível em `http://localhost:5000/swagger`

### Executando localmente

1. Configure o SQL Server:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
   ```

2. Execute as migrações:
   ```bash
   cd apiB2e
   dotnet ef database update
   ```

3. Execute o projeto:
   ```bash
   dotnet run
   ```

## API Endpoints

### Autenticação
- **POST /api/auth/login**: Autenticação de usuário

### Produtos
- **GET /api/produtos**: Lista produtos (paginado)
  - Parâmetros: page, pageSize, sortOrder
- **GET /api/produtos/{id}**: Obtém produto por ID
- **POST /api/produtos**: Cria novo produto
- **PUT /api/produtos/{id}**: Atualiza produto
- **DELETE /api/produtos/{id}**: Remove produto
- **GET /api/produtos/export**: Exporta produtos para Excel

## Usuários para Teste

1. **Admin**
   - Login: admin
   - Senha: admin123

2. **Teste**
   - Login: teste
   - Senha: teste123

## Recursos Implementados

- [x] Minimal APIs (.NET 8)
- [x] Entity Framework Core (Code First)
- [x] Migrações automáticas
- [x] Autenticação básica
- [x] Paginação e ordenação
- [x] Exportação Excel
- [x] Containerização com Docker
- [x] CORS configurado
- [x] Swagger/OpenAPI