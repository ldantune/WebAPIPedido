using AutoMapper;
using Moq;
using FluentAssertions;
using Xunit;
using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Enums;
using WebAPI.Pedidos.Repositories;
using WebAPI.Pedidos.Services;

namespace WebAPI.Tests.Services;
public class PedidoServiceTests
{
    private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PedidoService _pedidoService;

    public PedidoServiceTests()
    {
        _pedidoRepositoryMock = new Mock<IPedidoRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _mapperMock = new Mock<IMapper>();

        _pedidoService = new PedidoService(
            _pedidoRepositoryMock.Object,
            _produtoRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task IniciarPedidoAsync_Should_Return_PedidoDTO()
    {
        var pedido = new Pedido { Id = 1, Status = StatusPedido.Aberto, DataCriacao = DateTime.Now };
        _pedidoRepositoryMock.Setup(repo => repo.IniciarPedidoAsync()).ReturnsAsync(pedido);
        _mapperMock.Setup(mapper => mapper.Map<PedidoDTO>(pedido)).Returns(new PedidoDTO { Id = 1, Status = StatusPedido.Aberto });

        var result = await _pedidoService.IniciarPedidoAsync();

        result.Should().BeOfType<PedidoDTO>();
        result.Id.Should().Be(1);
        result.Status.Should().Be(StatusPedido.Aberto);
    }

    [Fact]
    public async Task AdicionarProdutoAoPedidoAsync_Should_Add_Produto()
    {
        var pedidoId = 1;
        var produtoDto = new PedidoProdutoDTO { ProdutoId = 1, Quantidade = 2 };
        var pedido = new Pedido
        {
            Id = 1,
            Status = StatusPedido.Aberto,
            Produtos = new List<PedidoProduto>()
        };
        var produto = new Produto
        {
            Id = 1,
            Estoque = 10,
            Preco = 50
        };

        _pedidoRepositoryMock.Setup(repo => repo.ObterPedidoPorIdAsync(pedidoId)).ReturnsAsync(pedido);
        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(produtoDto.ProdutoId)).ReturnsAsync(produto);
        _pedidoRepositoryMock.Setup(repo => repo.AdicionarProdutoAoPedidoAsync(pedidoId, It.IsAny<PedidoProduto>()))
            .Callback<int, PedidoProduto>((id, pedidoProduto) =>
            {
                pedido.Produtos.Add(pedidoProduto);
            });

        var result = await _pedidoService.AdicionarProdutoAoPedidoAsync(pedidoId, produtoDto);

        result.Should().NotBeNull();  
        result.Produtos.Should().HaveCount(1);  
        result.Produtos[0].ProdutoId.Should().Be(1); 
        result.Produtos[0].Quantidade.Should().Be(2);  
        result.Produtos[0].PrecoUnitario.Should().Be(50); 
    }


    [Fact]
    public async Task RemoverProdutoDoPedidoAsync_Should_Remove_Produto()
    {
        var pedidoId = 1;
        var produtoId = 1;
        var pedido = new Pedido
        {
            Id = 1,
            Status = StatusPedido.Aberto,
            Produtos = new List<PedidoProduto>
            {
                new PedidoProduto { ProdutoId = 1, Quantidade = 1, PrecoUnitario = 50 }
            }
        };

        _pedidoRepositoryMock.Setup(repo => repo.ObterPedidoPorIdAsync(pedidoId)).ReturnsAsync(pedido);
        _pedidoRepositoryMock.Setup(repo => repo.RemoverProdutoDoPedidoAsync(pedidoId, It.IsAny<PedidoProduto>()));

        var result = await _pedidoService.RemoverProdutoDoPedidoAsync(pedidoId, produtoId);

        result.Should().NotBeNull();
        result.Produtos.Should().BeEmpty();
    }

    [Fact]
    public async Task FecharPedidoAsync_Should_Close_Pedido()
    {
        var pedidoId = 1;
        var pedido = new Pedido {
                Id = 1,
                Status = StatusPedido.Aberto,
                Produtos = new List<PedidoProduto>
                {
                    new PedidoProduto { ProdutoId = 1, Quantidade = 1, PrecoUnitario = 50 }
                }
            };

        _pedidoRepositoryMock.Setup(repo => repo.ObterPedidoPorIdAsync(pedidoId)).ReturnsAsync(pedido);

        _produtoRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new Produto { Id = 1, Nome = "Produto 1", Preco = 50, Estoque = 10 });

        _pedidoRepositoryMock.Setup(repo => repo.AtualizarPedidoAsync(It.IsAny<Pedido>()))
                             .Callback<Pedido>(p => p.Status = StatusPedido.Fechado);

        var result = await _pedidoService.FecharPedidoAsync(pedidoId);

        result.Status.Should().Be(StatusPedido.Fechado);
        _pedidoRepositoryMock.Verify(repo => repo.AtualizarPedidoAsync(It.IsAny<Pedido>()), Times.Once);
    }


    [Fact]
    public async Task ListarPedidosAsync_Should_Return_List_Of_Pedidos()
    {
        var pedidos = new List<Pedido>
        {
            new Pedido { Id = 1, Status = StatusPedido.Aberto, DataCriacao = DateTime.Now }
        };

        _pedidoRepositoryMock.Setup(repo => repo.ListarPedidosAsync(1, 10, null)).ReturnsAsync(pedidos);

        var result = await _pedidoService.ListarPedidosAsync(1, 10, null);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(1);
        result[0].Status.Should().Be(StatusPedido.Aberto);
    }

    [Fact]
    public async Task ObterPedidoPorIdAsync_Should_Return_PedidoDTO()
    {
        var pedidoId = 1;
        var pedido = new Pedido { Id = 1, Status = StatusPedido.Aberto, DataCriacao = DateTime.Now, Produtos = new List<PedidoProduto>() };
        _pedidoRepositoryMock.Setup(repo => repo.ObterPedidoPorIdAsync(pedidoId)).ReturnsAsync(pedido);

        var result = await _pedidoService.ObterPedidoPorIdAsync(pedidoId);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Status.Should().Be(StatusPedido.Aberto);
    }
}
