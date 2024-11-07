using Microsoft.EntityFrameworkCore;
using WebAPI.Pedidos.Context;
using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Enums;

namespace WebAPI.Pedidos.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly ApplicationDbContext _context;

    public PedidoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido> IniciarPedidoAsync()
    {
        var pedido = new Pedido
        {
            DataCriacao = DateTime.Now,
            Status = StatusPedido.Aberto,
            Produtos = new List<PedidoProduto>()
        };

        await _context.Pedidos.AddAsync(pedido);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao salvar o pedido");
            throw;
        }

        return pedido;
    }

    public async Task<Pedido> AdicionarProdutoAoPedidoAsync(int pedidoId, PedidoProduto pedidoProduto)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Produtos)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null)
            throw new KeyNotFoundException("Pedido não encontrado.");

        if (pedido.Produtos.Any(pp => pp.ProdutoId == pedidoProduto.ProdutoId))
            throw new InvalidOperationException("Produto já está no pedido.");

        var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == pedidoProduto.ProdutoId);

        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado.");

        if (produto.Estoque < pedidoProduto.Quantidade)
            throw new InvalidOperationException("Estoque insuficiente para adicionar o produto ao pedido.");

        pedido.Produtos.Add(pedidoProduto);

        produto.Estoque -= pedidoProduto.Quantidade;

        if (StatusPedido.Aberto.Equals(pedido.Status))
        {
            pedido.Status = StatusPedido.EmAndamento;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao adicionar produto ao pedido");
            throw;
        }

        return pedido;
    }

    public async Task<Pedido> RemoverProdutoDoPedidoAsync(int pedidoId, PedidoProduto pedidoProduto)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Produtos)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null)
            throw new KeyNotFoundException("Pedido não encontrado.");

        var produto = pedido.Produtos.FirstOrDefault(pp => pp.ProdutoId == pedidoProduto.ProdutoId);
        if (produto == null)
            throw new KeyNotFoundException("Produto não encontrado no pedido.");

        pedido.Produtos.Remove(produto);

        var produtoEstoque = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == pedidoProduto.ProdutoId);
        if (produto != null)
        {
            // Devolve a quantidade ao estoque
            produtoEstoque.Estoque += produto.Quantidade;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao remover produto ao pedido");
        }

        return pedido;
    }

    public async Task<Pedido> AtualizarPedidoAsync(Pedido pedido)
    {
        var pedidoExistente = await _context.Pedidos
            .Include(p => p.Produtos)
            .FirstOrDefaultAsync(p => p.Id == pedido.Id);

        if (pedidoExistente == null)
            throw new KeyNotFoundException("Pedido não encontrado.");


        pedidoExistente.DataCriacao = pedido.DataCriacao;
        pedidoExistente.Status = pedido.Status;
        pedidoExistente.Produtos = pedido.Produtos;

       
        _context.Pedidos.Update(pedidoExistente);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao atualizar o pedido");
        }

        return pedidoExistente;
    }

    public async Task<Pedido> FecharPedidoAsync(int pedidoId)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Produtos)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null)
            throw new KeyNotFoundException("Pedido não encontrado.");

        if (StatusPedido.Fechado.Equals(pedido.Status))
            throw new InvalidOperationException("Pedido já está fechado.");

        if (!pedido.Produtos.Any())
            throw new InvalidOperationException("Pedido não pode ser fechado sem produtos.");

        foreach (var produto in pedido.Produtos)
        {
            var estoqueProduto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produto.ProdutoId);

            if (estoqueProduto == null)
                throw new KeyNotFoundException("Produto não encontrado.");
        }

        pedido.Status = StatusPedido.Fechado;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao fechar o pedido");
        }

        return pedido;
    }

    public async Task<List<Pedido>> ListarPedidosAsync(int paginaAtual, int itensPorPagina, StatusPedido? status)
    {
        if (paginaAtual <= 0) paginaAtual = 1;
        if (itensPorPagina <= 0) itensPorPagina = 3;

        var query = _context.Pedidos
            .Include(p => p.Produtos)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        return await query
            .Skip((paginaAtual - 1) * itensPorPagina)
            .Take(itensPorPagina)
            .ToListAsync();
    }

    public async Task<Pedido> ObterPedidoPorIdAsync(int pedidoId)
    {
        return await _context.Pedidos
            .Include(p => p.Produtos)
            .ThenInclude(pp => pp.Produto)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);
    }
}
