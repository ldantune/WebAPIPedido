using AutoMapper;
using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Enums;
using WebAPI.Pedidos.Repositories;

namespace WebAPI.Pedidos.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidoRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public PedidoService(IPedidoRepository pedidoRepository, IProdutoRepository produtoRepository, IMapper mapper)
    {
        _pedidoRepository = pedidoRepository;
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<PedidoDTO> IniciarPedidoAsync()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();
        var pedidoDto = _mapper.Map<PedidoDTO>(pedido);
        return pedidoDto;
    }

    public async Task<PedidoDTO> AdicionarProdutoAoPedidoAsync(int pedidoId, PedidoProdutoDTO pedidoProdutoDto)
    {
        var pedido = await _pedidoRepository.ObterPedidoPorIdAsync(pedidoId);
        if (pedido == null)
            throw new InvalidOperationException("Pedido não encontrado.");

        if (StatusPedido.Fechado.Equals(pedido.Status))
            throw new InvalidOperationException("Pedido está fechado.");

        var produto = await _produtoRepository.GetByIdAsync(pedidoProdutoDto.ProdutoId);
        if (produto == null)
            throw new InvalidOperationException("Produto não encontrado.");

        if (produto.Estoque < pedidoProdutoDto.Quantidade)
            throw new InvalidOperationException("Estoque insuficiente para adicionar o produto.");

        var pedidoProduto = new PedidoProduto
        {
            ProdutoId = pedidoProdutoDto.ProdutoId,
            Quantidade = pedidoProdutoDto.Quantidade,
            PrecoUnitario = produto.Preco
        };

        await _pedidoRepository.AdicionarProdutoAoPedidoAsync(pedidoId, pedidoProduto);

        decimal valorTotal = pedido.Produtos.Sum(p => p.Quantidade * p.PrecoUnitario);

        return new PedidoDTO
        {
            Id = pedido.Id,
            DataCriacao = pedido.DataCriacao,
            Status = pedido.Status,
            ValorTotal = valorTotal,
            Produtos = pedido.Produtos.Select(p => new PedidoProdutoDTO
            {
                ProdutoId = p.ProdutoId,
                Quantidade = p.Quantidade,
                NomeProduto = p.Produto?.Nome,
                PrecoUnitario = p.PrecoUnitario
            }).ToList()
        };
    }

    public async Task<PedidoDTO> RemoverProdutoDoPedidoAsync(int pedidoId, int produtoId)
    {
        var pedido = await _pedidoRepository.ObterPedidoPorIdAsync(pedidoId);
        if (pedido == null)
            throw new InvalidOperationException("Pedido não encontrado.");

        if (StatusPedido.Fechado.Equals(pedido.Status))
            throw new InvalidOperationException("Pedido já está fechado.");

        var pedidoProduto = pedido.Produtos.FirstOrDefault(p => p.ProdutoId == produtoId);
        if (pedidoProduto == null)
            throw new InvalidOperationException("Produto não encontrado no pedido.");

        await _pedidoRepository.RemoverProdutoDoPedidoAsync(pedidoId, pedidoProduto);

        return new PedidoDTO
        {
            Id = pedido.Id,
            DataCriacao = pedido.DataCriacao,
            Status = pedido.Status
        };
    }

    public async Task<PedidoDTO> FecharPedidoAsync(int pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPedidoPorIdAsync(pedidoId);

        if (pedido == null)
            throw new InvalidOperationException("Pedido não encontrado.");

        if (StatusPedido.Fechado.Equals(pedido.Status))
            throw new InvalidOperationException("Pedido já está fechado.");

        if (!pedido.Produtos.Any())
            throw new InvalidOperationException("Pedido não pode ser fechado sem produtos.");

        foreach (var pedidoProduto in pedido.Produtos)
        {
            var produto = await _produtoRepository.GetByIdAsync(pedidoProduto.ProdutoId);

            if (produto == null)
                throw new InvalidOperationException("Produto não encontrado.");
        }

        pedido.Status = StatusPedido.Fechado;

        await _pedidoRepository.AtualizarPedidoAsync(pedido);

        decimal valorTotal = pedido.Produtos.Sum(p => p.Quantidade * p.PrecoUnitario);

        return new PedidoDTO
        {
            Id = pedido.Id,
            DataCriacao = pedido.DataCriacao,
            Status = pedido.Status,
            ValorTotal = valorTotal,
            Produtos = pedido.Produtos.Select(p => new PedidoProdutoDTO
            {
                ProdutoId = p.ProdutoId,
                Quantidade = p.Quantidade,
                NomeProduto = p.Produto?.Nome,
                PrecoUnitario = p.PrecoUnitario
            }).ToList()
        };
    }

    public async Task<List<PedidoDTO>> ListarPedidosAsync(int paginaAtual, int itensPorPagina, StatusPedido? status)
    {
        var pedidos = await _pedidoRepository.ListarPedidosAsync(paginaAtual, itensPorPagina, status);

        var pedidosDto = new List<PedidoDTO>();
        foreach (var pedido in pedidos)
        {
            decimal valorTotal = pedido.Produtos.Sum(p => p.Quantidade * p.PrecoUnitario);
            pedidosDto.Add(new PedidoDTO
            {
                Id = pedido.Id,
                DataCriacao = pedido.DataCriacao,
                Status = pedido.Status,
                ValorTotal = valorTotal
            });
        }
        return pedidosDto;
    }

    public async Task<PedidoDTO> ObterPedidoPorIdAsync(int pedidoId)
    {
        var pedido = await _pedidoRepository.ObterPedidoPorIdAsync(pedidoId);

        if (pedido == null)
        {
            throw new KeyNotFoundException($"Pedido com ID {pedidoId} não foi encontrado.");
        }

        decimal valorTotal = pedido.Produtos.Sum(p => p.Quantidade * p.PrecoUnitario);

        return new PedidoDTO
        {
            Id = pedido.Id,
            DataCriacao = pedido.DataCriacao,
            Status = pedido.Status,
            ValorTotal = valorTotal,
            Produtos = pedido.Produtos.Select(p => new PedidoProdutoDTO
            {
                ProdutoId = p.ProdutoId,
                Quantidade = p.Quantidade,
                NomeProduto = p.Produto?.Nome,
                PrecoUnitario = p.PrecoUnitario
            }).ToList()
        };
    }

}
