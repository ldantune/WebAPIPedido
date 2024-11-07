using WebAPI.Pedidos.Entities;

namespace WebAPI.Pedidos.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> GetAll(int paginaAtual, int itensPorPagina);
    Task<Produto> GetByIdAsync(int id);
    Task<Produto> Create(Produto produto);
    Task UpdateAsync(Produto produto);
}
