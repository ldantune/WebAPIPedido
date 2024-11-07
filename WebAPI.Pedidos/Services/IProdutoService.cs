using WebAPI.Pedidos.DTOs;

namespace WebAPI.Pedidos.Services;

public interface IProdutoService
{
    Task<IEnumerable<ProdutoDTO>> GetProdutos(int paginaAtual, int itensPorPagina);
    Task AddProduto(ProdutoDTO produtoDTO);
}
