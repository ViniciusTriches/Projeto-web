# Arquitetura da Aplicação

## Visão Geral

```
┌─────────────────────────────────────────────────────────┐
│                   Browser (Usuário)                     │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTP
┌──────────────────────────▼──────────────────────────────┐
│             Marketplace.Web.MVC (ASP.NET Core 9)        │
│  ┌─────────────────────────────────────────────────────┐ │
│  │  Controllers  (HTTP → lógica de apresentação)       │ │
│  │  HomeController · ProdutosController                │ │
│  │  ContaController · CarrinhoController               │ │
│  │  CheckoutController · PedidosController             │ │
│  │  EstatisticasController                             │ │
│  └──────────────────┬──────────────────────────────────┘ │
│                     │ IMarketplaceFacade                  │
│  ┌──────────────────▼──────────────────────────────────┐ │
│  │           MarketplaceFacade (Padrão Façade)         │ │
│  │  Orquestra chamadas paralelas, encapsula sequências │ │
│  └──┬──────┬──────┬──────┬──────┬──────┬────────────┬─┘ │
│     │      │      │      │      │      │            │    │
│  Usr  Prod  Cat  Ped  Pag  Aval  Estat  JwtDecoder  │    │
│  ─────────────────────────────────────────────────  │    │
│            Typed HttpClients (DI)                   │    │
└─────────────────────────────────────────────────────┘    │
           │ HTTP REST para cada microserviço               │
┌──────────▼──────────────────────────────────────────────┐
│                Microserviços (neurosky.com.br)           │
│  Usuários · Produtos · Categorias · Pedidos              │
│  Pagamentos · Avaliações · Estatísticas                  │
└─────────────────────────────────────────────────────────┘
```

## Padrão Façade

O padrão Façade é implementado em `src/Marketplace.Web.MVC/Services/Facades/`.

**Por que Façade aqui?**
- Centraliza toda a integração com os 7 microserviços em um único ponto
- Controllers dependem apenas de `IMarketplaceFacade` — não de nenhum client HTTP individual
- Permite chamadas paralelas (`Task.WhenAll`) transparentes para os controllers
- Facilita substituição e mock de qualquer microserviço sem alterar controllers

**Exemplo de orquestração paralela (produto + avaliações):**
```csharp
var (produto, avaliacoes) = await (
    produtos.ObterPorIdAsync(produtoId),
    avaliacoes.ListarPorProdutoAsync(produtoId)
).WhenAll();
```

## Autenticação

- Cookie Authentication (ASP.NET Core)
- JWT decodificado sem validação de assinatura (apenas leitura de claims)
- Token armazenado como Claim `"AccessToken"` no cookie
- Claims extraídos: `sub`, `name`, `email`, `role`

## Carrinho de Compras

- Sessão de carrinho via `IDistributedCache` (MemoryCache)
- Chave: `carrinho_{sessionId}` — isolado por usuário/aba
- Extensível para Redis sem alterar o controller

## Microserviços Consumidos

| Serviço | Base URL | Repositório |
|---|---|---|
| Usuários | http://usuario.neurosky.com.br | Marketplace.Auth.API |
| Produtos | http://produto.neurosky.com.br | marketplace-microservices |
| Categorias | http://categoria.neurosky.com.br | CategoriaService |
| Pedidos | http://pedido.neurosky.com.br | TrabalhoCarrinho |
| Pagamentos | http://pagamento.neurosky.com.br | API_PagamentoFrete |
| Avaliações | http://avaliacao.neurosky.com.br | ProdutoAvaliacao |
| Estatísticas | http://estatistica.neurosky.com.br | TabelasEstatitisca |
