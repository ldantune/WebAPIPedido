using Microsoft.EntityFrameworkCore;
using WebAPI.Pedidos.Context;
using WebAPI.Pedidos.Entities;

namespace WebAPI.Pedidos.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly ApplicationDbContext _context;

    public ProdutoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Produto>> GetAll(int paginaAtual, int itensPorPagina)
    {
        if (paginaAtual <= 0) paginaAtual = 1;
        if (itensPorPagina <= 0) itensPorPagina = 3;

        return await _context.Produtos
            .Skip((paginaAtual - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync();
    }

    public async Task<Produto> GetByIdAsync(int id)
    {
        return await _context.Produtos.FindAsync(id);
    }

    public async Task<Produto> Create(Produto produto)
    {
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return produto;
    }

    public async Task UpdateAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }

}
