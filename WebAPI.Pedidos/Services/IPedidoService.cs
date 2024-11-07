using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Enums;

namespace WebAPI.Pedidos.Services;

public interface IPedidoService
{
    Task<PedidoDTO> IniciarPedidoAsync();
    Task<PedidoDTO> AdicionarProdutoAoPedidoAsync(int pedidoId, PedidoProdutoDTO pedidoProdutoDto);
    Task<PedidoDTO> RemoverProdutoDoPedidoAsync(int pedidoId, int produtoId);
    Task<PedidoDTO> FecharPedidoAsync(int pedidoId);
    Task<List<PedidoDTO>> ListarPedidosAsync(int paginaAtual, int itensPorPagina, StatusPedido? status);
    Task<PedidoDTO> ObterPedidoPorIdAsync(int pedidoId);
}
