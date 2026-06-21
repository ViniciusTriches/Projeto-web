# Marketplace.Web.MVC

Interface web de um marketplace com arquitetura de microserviços, desenvolvida para a disciplina de **Projetos de Sistemas Web** (Uniftec).

## Tecnologias

| Tecnologia | Uso |
|---|---|
| ASP.NET Core 10 MVC | Framework principal |
| Bootstrap 5 + Bootstrap Icons | UI responsiva |
| C# / .NET 10 | Linguagem e runtime |
| HttpClient tipado | Comunicação com as APIs REST |
| Cookie Authentication | Sessão do usuário |
| JWT (decode sem validar assinatura) | Leitura de claims do token |
| IDistributedCache (MemoryCache) | Carrinho de compras em sessão |
| Docker | Containerização |
| Render | Deploy em cloud |

## Arquitetura

Os controllers dependem apenas de `IMarketplaceFacade`, que centraliza toda a integração com os microserviços. Isso mantém os controllers limpos e permite que a Façade faça chamadas paralelas quando necessário.

## Microserviços Consumidos

| Microserviço | URL | Repositório |
|---|---|---|
| Usuários / Auth | http://usuario.neurosky.com.br | [Marketplace.Auth.API](https://github.com/RonaldoGrillo/Marketplace.Auth.API) |
| Produtos | http://produto.neurosky.com.br | [marketplace-microservices](https://github.com/Leo-Trevisol/marketplace-microservices) |
| Categorias | http://categoria.neurosky.com.br | [CategoriaService](https://github.com/LauraUberti/CategoriaService) |
| Pedidos / Carrinho | http://pedido.neurosky.com.br | [TrabalhoCarrinho](https://github.com/DiogoTissian/ftec.projetoweb.TrabalhoCarrinho.sln) |
| Pagamentos / Frete | http://pagamento.neurosky.com.br | [API_PagamentoFrete](https://github.com/Arthur-Cavali/API_PagamentoFrete) |
| Avaliações | http://avaliacao.neurosky.com.br | [ProdutoAvaliacao](https://github.com/GCunico/Ftec.ProjetosWeb.ProdutoAvaliacao) |
| Estatísticas | http://estatistica.neurosky.com.br | [TabelasEstatitisca](https://github.com/juliaanesantos/TabelasEstatitisca) |

## Como Executar

```bash
cd src/Marketplace.Web.MVC
dotnet restore
dotnet run
```

Para instruções detalhadas (Docker, Render, variáveis de ambiente): [docs/execucao.md](docs/execucao.md)

## Documentação

- [Arquitetura e padrões](docs/arquitetura.md)
- [Endpoints consumidos](docs/endpoints-consumidos.md)
- [Execução e deploy](docs/execucao.md)

## Integrantes

- **Vinicius Triches**
- **Ronaldo Grillo**
