# Endpoints Consumidos

## Usuários / Auth
Base: `http://usuario.neurosky.com.br`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| POST | `/api/usuario/login` | Login — retorna JWT | Não |
| POST | `/api/usuario` | Criar conta | Não |
| GET | `/api/usuario/{id}` | Obter perfil | Bearer |
| PUT | `/api/usuario/{id}` | Atualizar perfil | Bearer |

## Produtos
Base: `http://produto.neurosky.com.br`

Respostas envolvidas em wrapper `{ sucesso, data, message }`.

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| GET | `/api/produto/listar` | Listar todos | Não |
| GET | `/api/produto/buscar/{texto}` | Buscar por texto | Não |
| GET | `/api/produto/obtemPorId/{id}` | Obter por ID | Não |
| GET | `/api/produto/obtemPorCodigo/{codigo}` | Obter por código | Não |

> ⚠️ `ProdutoDto.IdCategoria` é `Guid` e `CategoriaDto.Id` é `int` — tipos incompatíveis, não fazer join.

## Categorias
Base: `http://categoria.neurosky.com.br`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| GET | `/api/categoria` | Listar todas | Não |
| GET | `/api/categoria/{id}` | Obter por ID (int) | Não |

## Pedidos / Carrinho
Base: `http://pedido.neurosky.com.br`

O controller da API se chama `Carrinho`, não `Pedido`.

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| GET | `/api/carrinho/{usuarioId}` | Listar pedidos do usuário | Bearer |
| PUT | `/api/carrinho/atualizarstatus` | Atualizar status do pedido | Bearer |
| DELETE | `/api/carrinho/{usuarioId}` | Limpar carrinho do usuário | Bearer |
| DELETE | `/api/carrinho/pedido/{pedidoId}` | Deletar pedido específico | Bearer |

## Pagamentos / Frete
Base: `http://pagamento.neurosky.com.br`

Valores monetários em `double` (não `decimal`).

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| GET | `/api/pagamento/transportadoras` | Listar transportadoras | Não |
| POST | `/api/pagamento/calcularfrete` | Calcular frete | Bearer |
| POST | `/api/pagamento` | Criar pagamento | Bearer |
| POST | `/api/pagamento/{pagamentoId}/processar` | Processar transação | Bearer |

## Avaliações
Base: `http://avaliacao.neurosky.com.br`

| Método | Rota | Descrição | Auth |
|---|---|---|---|
| GET | `/api/avaliacao/produto/{idProduto}` | Listar avaliações | Não |
| POST | `/api/avaliacao` | Criar avaliação | Bearer |

## Estatísticas
Base: `http://estatistica.neurosky.com.br`

> ⚠️ Serviço em desenvolvimento — cliente configurado com timeout de 5s e fallback gracioso.

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Estatistica/painel-hoje` | Painel de hoje |
