# DevOps Recipe API

Este repositório contém duas versões de uma API para gerenciamento de receitas e ingredientes, como pré-tarefa para o curso presencial de DevOps da Lacuna.

## Funcionalidades

- CRUD de ingredientes: nome e unidade de medida
- CRUD de receitas: nome, modo de preparo e ingredientes com quantidade
- Uso de banco de dados SQLite
- Serialização segura com DTOs (C#) e Pydantic (Python)

## Tecnologias Utilizadas

### C# (.NET)
- ASP.NET Core 7
- Entity Framework Core
- SQLite
- Swagger
- DTOs para entrada e saída

### Python
- FastAPI
- SQLAlchemy
- SQLite
- Pydantic
- Uvicorn
  
## Como rodar

### Versão C#

1. Navegue até a pasta:

```bash
cd csharp/ReceitasAPI
```
2. Restaure os pacotes e rode o servidor:

```bash
dotnet restore
dotnet run
```
3. Acesse no navegador:
```bash
http://localhost:5007/swagger
```

--- 

### Versão Python

1. Instale as dependências:
   
```bash
pip install -r requirements.txt
```

2. Rode o servidor:
   
```bash
uvicorn main:app --reload
```
3. Acesse no navegador:
   
```bash
http://localhost:8000/docs
```
