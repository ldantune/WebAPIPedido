# WebAPIPedido

## Projeto .NET

# Rotas implementadas para pedido
    IniciarPedido (Iniciar um novo pedido)
    AdicionarProdutoAoPedido (Adicionar produtos ao pedido)
    RemoverProdutoDoPedido (Remover produtos do pedido)
    FecharPedido (Fechar o pedido)
    ListarPedidos (Listar os pedidos, com paginação e por status)
    ObterPedido(Obter um pedido (e seu produtos) através do ID)

# Rotas implementadas para produtos
    Get (Buscar todos os produtos, com paginação)
    Post (Cadastrar um novo produto)

# Observação: Caso o projeto não tenha a pasta de migrations, ela pode ser criada através do comando:
    dotnet ef migrations add InitialMigration

# Após a criação da migration, execute o comando a seguir para atualizar o banco de dados e criar as tabelas:**
    dotnet ef database update

Neste projeto, o banco de dados utilizado é o SQLite.

