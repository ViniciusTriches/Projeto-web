# Execução e Deploy

## Requisitos

- .NET 9 SDK ou superior
- Acesso à internet (para consumir as APIs em neurosky.com.br)

## Execução Local

```bash
git clone https://github.com/ViniciusTriches/Projeto-web.git
cd Projeto-web/src/Marketplace.Web.MVC
dotnet restore
dotnet run
```

A aplicação estará disponível em `http://localhost:5000` (ou porta exibida no terminal).

### Variáveis de Configuração

As URLs dos microserviços estão em `src/Marketplace.Web.MVC/appsettings.json`:

```json
{
  "Microservicos": {
    "Usuarios":    "http://usuario.neurosky.com.br",
    "Produtos":    "http://produto.neurosky.com.br",
    "Categorias":  "http://categoria.neurosky.com.br",
    "Pedidos":     "http://pedido.neurosky.com.br",
    "Pagamentos":  "http://pagamento.neurosky.com.br",
    "Avaliacoes":  "http://avaliacao.neurosky.com.br",
    "Estatisticas":"http://estatistica.neurosky.com.br"
  }
}
```

## Docker

O projeto contém um `Dockerfile` multi-stage em `src/Marketplace.Web.MVC/Dockerfile`.

```bash
docker build -t marketplace-web -f src/Marketplace.Web.MVC/Dockerfile .
docker run -p 8080:8080 marketplace-web
```

## Deploy — Render

A aplicação é hospedada na plataforma [Render](https://render.com).

URL de produção: `[URL_AQUI]`

### Configuração no Render

1. Conectar o repositório GitHub `ViniciusTriches/Projeto-web`
2. Tipo: **Web Service**
3. Runtime: **Docker**
4. Dockerfile path: `src/Marketplace.Web.MVC/Dockerfile`
5. Variáveis de ambiente: adicionar as chaves `Microservicos__*` conforme necessário

## Usuários de Teste

Criar uma conta pelo formulário de cadastro em `/Conta/Cadastro`. O microserviço de usuários exige:
- Nome completo
- E-mail válido
- Senha com mínimo 8 caracteres, 1 maiúscula, 1 número, 1 caractere especial
- CPF/CNPJ (Documento)
