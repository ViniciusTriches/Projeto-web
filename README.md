# Marketplace.Web.MVC

Interface web integrada de um marketplace construído com arquitetura de microserviços, desenvolvido para a disciplina de **Projetos de Sistemas Web** (Uniftec).

## Tecnologias

| Tecnologia | Uso |
|---|---|
| ASP.NET Core 9 MVC | Framework principal da interface web |
| Bootstrap 5 | Estilização e componentes responsivos |
| Bootstrap Icons | Iconografia |
| C# / .NET 9 | Linguagem e runtime |
| HttpClient tipado | Comunicação com as APIs REST dos microserviços |
| Cookie Authentication | Autenticação do usuário na interface |
| JWT (decode sem validar assinatura) | Leitura dos claims do token emitido pelo microserviço de Auth |
| IDistributedCache (Memory) | Carrinho de compras em sessão |
| Docker | Containerização para deploy em cloud |
| Render | Plataforma de cloud utilizada para o deploy |

## Padrões de Projeto Aplicados

### MVC (Model-View-Controller)
A interface segue rigorosamente o padrão MVC do ASP.NET Core. Os **Controllers** recebem as requisições HTTP, orquestram a lógica de apresentação (via Façade) e escolhem qual **View** renderizar. Os **Models** transportam dados entre as camadas. As **Views** (Razor `.cshtml`) são responsáveis exclusivamente pela apresentação — nenhuma lógica de negócio reside nelas.

### Façade
A camada `Services/Facades/MarketplaceFacade` unifica o acesso a todos os microserviços. Os Controllers **nunca** injetam os clients HTTP individuais (`IProdutosClient`, `ICategoriasClient`, etc.) — eles dependem apenas de `IMarketplaceFacade`. A Façade é responsável por:
- Orquestrar chamadas paralelas a múltiplos serviços (ex: produto + avaliações em paralelo)
- Encapsular a sequência de operações do checkout (criar pedido → frete → pagamento → transação)
- Abstrair os detalhes de cada microserviço dos Controllers

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

Consulte [docs/execucao.md](docs/execucao.md) para instruções detalhadas de execução local e deploy.

**Resumo rápido:**
```bash
cd src/Marketplace.Web.MVC
dotnet restore
dotnet run
```

## Documentação

- [Arquitetura e padrões](docs/arquitetura.md)
- [Endpoints consumidos](docs/endpoints-consumidos.md)
- [Execução local e deploy](docs/execucao.md)

## Integrantes

- **Vinicius Triches**
- **Ronaldo Grillo**
