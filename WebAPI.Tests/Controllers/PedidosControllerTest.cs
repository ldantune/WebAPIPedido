using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Pedidos.Controllers;
using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Enums;
using WebAPI.Pedidos.Services;

namespace WebAPI.Tests.Controllers;
public class PedidosControllerTest
{
    private readonly PedidosController _pedidosController;
    private readonly Mock<IPedidoService> _pedidoServiceMock;
    private readonly Mock<IMapper> _mapperMock;

    public PedidosControllerTest()
    {
        _pedidoServiceMock = new Mock<IPedidoService>();
        _mapperMock = new Mock<IMapper>();
        _pedidosController = new PedidosController(_pedidoServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task IniciarPedido_Should_Return_Created()
    {
        var pedidoDto = new PedidoDTO { Id = 1, Status = StatusPedido.Aberto };
        _pedidoServiceMock.Setup(service => service.IniciarPedidoAsync()).ReturnsAsync(pedidoDto);

        var result = await _pedidosController.IniciarPedido();

        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult.StatusCode.Should().Be(201);
        createdAtActionResult.RouteValues["id"].Should().Be(pedidoDto.Id);
        createdAtActionResult.Value.Should().BeEquivalentTo(pedidoDto);
    }

    [Fact]
    public async Task AdicionarProdutoAoPedido_Should_Return_OK()
    {
        var pedidoId = 1;
        var produtoDto = new PedidoProdutoDTO { ProdutoId = 1, Quantidade = 2 };
        var pedidoDto = new PedidoDTO { Id = pedidoId, Status = StatusPedido.Aberto, Produtos = new List<PedidoProdutoDTO> { produtoDto } };

        _pedidoServiceMock.Setup(service => service.AdicionarProdutoAoPedidoAsync(pedidoId, produtoDto)).ReturnsAsync(pedidoDto);

        var result = await _pedidosController.AdicionarProdutoAoPedido(pedidoId, produtoDto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);  // Ok
        okResult.Value.Should().BeEquivalentTo(pedidoDto);  // Verifica se o pedido retornado é o esperado
    }

    [Fact]
    public async Task RemoverProdutoDoPedido_Should_Return_OK()
    {
        var pedidoId = 1;
        var produtoId = 1;
        var pedidoDto = new PedidoDTO { Id = pedidoId, Status = StatusPedido.Aberto };

        _pedidoServiceMock.Setup(service => service.RemoverProdutoDoPedidoAsync(pedidoId, produtoId)).ReturnsAsync(pedidoDto);

        var result = await _pedidosController.RemoverProdutoDoPedido(pedidoId, produtoId);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);  // Ok
        okResult.Value.Should().BeEquivalentTo(pedidoDto);  // Verifica se o pedido retornado é o esperado
    }

    [Fact]
    public async Task FecharPedido_Should_Return_OK()
    {
        var pedidoId = 1;
        var pedidoDto = new PedidoDTO { Id = pedidoId, Status = StatusPedido.Fechado };

        _pedidoServiceMock.Setup(service => service.FecharPedidoAsync(pedidoId)).ReturnsAsync(pedidoDto);

        var result = await _pedidosController.FecharPedido(pedidoId);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(pedidoDto);
    }

    [Fact]
    public async Task ListarPedidos_Should_Return_OK()
    {
        var pedidosDto = new List<PedidoDTO>
        {
            new PedidoDTO { Id = 1, Status = StatusPedido.Aberto },
            new PedidoDTO { Id = 2, Status = StatusPedido.Fechado }
        };

        _pedidoServiceMock.Setup(service => service.ListarPedidosAsync(1, 3, null)).ReturnsAsync(pedidosDto);

        var result = await _pedidosController.ListarPedidos(1, 3, null);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(pedidosDto);
    }

    [Fact]
    public async Task ObterPedido_Should_Return_OK_When_Found()
    {
        var pedidoId = 1;
        var pedidoDto = new PedidoDTO { Id = pedidoId, Status = StatusPedido.Aberto };

        _pedidoServiceMock.Setup(service => service.ObterPedidoPorIdAsync(pedidoId)).ReturnsAsync(pedidoDto);

        var result = await _pedidosController.ObterPedido(pedidoId);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(pedidoDto);
    }

    [Fact]
    public async Task ObterPedido_Should_Return_NotFound_When_Not_Found()
    {
        // Arrange
        var pedidoId = 1;
        _pedidoServiceMock.Setup(service => service.ObterPedidoPorIdAsync(pedidoId)).ReturnsAsync((PedidoDTO)null);

        var result = await _pedidosController.ObterPedido(pedidoId);

        var notFoundResult = result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
    }
}
