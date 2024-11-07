using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Pedidos.Context;
using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Enums;
using WebAPI.Pedidos.Repositories;

namespace WebAPI.Tests.Repositories;
public class PedidoRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly PedidoRepository _pedidoRepository;

    public PedidoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _pedidoRepository = new PedidoRepository(_context);
    }

    [Fact]
    public async Task IniciarPedidoAsync_Should_Create_New_Pedido()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();

        pedido.Should().NotBeNull();
        pedido.Status.Should().Be(StatusPedido.Aberto);
        pedido.DataCriacao.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        pedido.Produtos.Should().BeEmpty();
    }

    [Fact]
    public async Task AdicionarProdutoAoPedidoAsync_Should_Add_Product_To_Pedido()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();
        var produto = new Produto { Id = 1, Nome = "Produto 1", Estoque = 10 };
        var pedidoProduto = new PedidoProduto { ProdutoId = 1, Quantidade = 2 };

        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();

        var updatedPedido = await _pedidoRepository.AdicionarProdutoAoPedidoAsync(pedido.Id, pedidoProduto);

        updatedPedido.Produtos.Should().ContainSingle(pp => pp.ProdutoId == 1 && pp.Quantidade == 2);
        produto.Estoque.Should().Be(8); // Estoque deve diminuir após adicionar o produto ao pedido
        updatedPedido.Status.Should().Be(StatusPedido.EmAndamento);
    }

    [Fact]
    public async Task RemoverProdutoDoPedidoAsync_Should_Remove_Product_From_Pedido()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();
        var produto = new Produto { Id = 1, Nome = "Produto 1", Estoque = 10 };
        var pedidoProduto = new PedidoProduto { ProdutoId = 1, Quantidade = 2 };

        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
        await _pedidoRepository.AdicionarProdutoAoPedidoAsync(pedido.Id, pedidoProduto);

        var updatedPedido = await _pedidoRepository.RemoverProdutoDoPedidoAsync(pedido.Id, pedidoProduto);

        updatedPedido.Produtos.Should().BeEmpty();
        produto.Estoque.Should().Be(10); 
    }

    [Fact]
    public async Task FecharPedidoAsync_Should_Close_Pedido()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();
        var produto = new Produto { Id = 1, Nome = "Produto 1", Estoque = 10 };
        var pedidoProduto = new PedidoProduto { ProdutoId = 1, Quantidade = 2 };

        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
        await _pedidoRepository.AdicionarProdutoAoPedidoAsync(pedido.Id, pedidoProduto);

        var closedPedido = await _pedidoRepository.FecharPedidoAsync(pedido.Id);

        closedPedido.Status.Should().Be(StatusPedido.Fechado);
    }

    [Fact]
    public async Task AtualizarPedidoAsync_Should_Update_Existing_Pedido()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();
        var produto = new Produto { Id = 1, Nome = "Produto 1", Estoque = 10 };
        var pedidoProduto = new PedidoProduto { ProdutoId = 1, Quantidade = 2 };

        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
        await _pedidoRepository.AdicionarProdutoAoPedidoAsync(pedido.Id, pedidoProduto);

        pedido.Status = StatusPedido.Fechado;

        var updatedPedido = await _pedidoRepository.AtualizarPedidoAsync(pedido);

        updatedPedido.Status.Should().Be(StatusPedido.Fechado);
    }

    [Fact]
    public async Task ListarPedidosAsync_Should_Return_Pedidos_With_Pagination()
    {
        var pedido1 = new Pedido { Status = StatusPedido.Aberto, DataCriacao = DateTime.Now };
        var pedido2 = new Pedido { Status = StatusPedido.Aberto, DataCriacao = DateTime.Now };
        var pedido3 = new Pedido { Status = StatusPedido.Aberto, DataCriacao = DateTime.Now };
        var pedido4 = new Pedido { Status = StatusPedido.Aberto, DataCriacao = DateTime.Now };

        await _context.Pedidos.AddRangeAsync(pedido1, pedido2, pedido3, pedido4);
        await _context.SaveChangesAsync();

        var result = await _pedidoRepository.ListarPedidosAsync(1, 2, StatusPedido.Aberto);

        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Id == pedido1.Id);
        result.Should().Contain(p => p.Id == pedido2.Id);
    }

    [Fact]
    public async Task ObterPedidoPorIdAsync_Should_Return_Pedido_With_Products()
    {
        var pedido = await _pedidoRepository.IniciarPedidoAsync();
        var produto = new Produto { Id = 1, Nome = "Produto 1", Estoque = 10 };
        var pedidoProduto = new PedidoProduto { ProdutoId = 1, Quantidade = 2 };

        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
        await _pedidoRepository.AdicionarProdutoAoPedidoAsync(pedido.Id, pedidoProduto);

        var result = await _pedidoRepository.ObterPedidoPorIdAsync(pedido.Id);

        result.Should().NotBeNull();
        result.Produtos.Should().ContainSingle(pp => pp.ProdutoId == 1);
    }
}
