using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Enums;

namespace WebAPI.Pedidos.Repositories;

public interface IPedidoRepository
{
    Task<Pedido> IniciarPedidoAsync();
    Task<Pedido> AdicionarProdutoAoPedidoAsync(int pedidoId, PedidoProduto pedidoProduto);
    Task<Pedido> RemoverProdutoDoPedidoAsync(int pedidoId, PedidoProduto pedidoProduto);
    Task<Pedido> AtualizarPedidoAsync(Pedido pedido);
    Task<Pedido> FecharPedidoAsync(int pedidoId);
    Task<List<Pedido>> ListarPedidosAsync(int paginaAtual, int itensPorPagina, StatusPedido? status);
    Task<Pedido> ObterPedidoPorIdAsync(int pedidoId);
}
