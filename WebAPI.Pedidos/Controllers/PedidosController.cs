using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Enums;
using WebAPI.Pedidos.Services;

namespace WebAPI.Pedidos.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidoService;
    private readonly IMapper _mapper;

    public PedidosController(IPedidoService pedidoService, IMapper mapper)
    {
        _pedidoService = pedidoService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> IniciarPedido()
    {
        try
        {
            var pedido = await _pedidoService.IniciarPedidoAsync();
            return CreatedAtAction(nameof(ObterPedido), new { id = pedido.Id }, pedido);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{pedidoId}/produtos")]
    public async Task<IActionResult> AdicionarProdutoAoPedido(int pedidoId, [FromBody] PedidoProdutoDTO pedidoProdutoDto)
    {
        try
        {
            var pedido = await _pedidoService.AdicionarProdutoAoPedidoAsync(pedidoId, pedidoProdutoDto);
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{pedidoId}/produtos/{produtoId}")]
    public async Task<IActionResult> RemoverProdutoDoPedido(int pedidoId, int produtoId)
    {
        try
        {
            var pedido = await _pedidoService.RemoverProdutoDoPedidoAsync(pedidoId, produtoId);
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{pedidoId}/fechar")]
    public async Task<IActionResult> FecharPedido(int pedidoId)
    {
        try
        {
            var pedido = await _pedidoService.FecharPedidoAsync(pedidoId);
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ListarPedidos([FromQuery] int paginaAtual = 1, [FromQuery] int itensPorPagina = 3, [FromQuery] StatusPedido? status = null)
    {
        try
        {
            var pedidos = await _pedidoService.ListarPedidosAsync(paginaAtual, itensPorPagina, status);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPedido(int id)
    {
        try
        {
            var pedido = await _pedidoService.ObterPedidoPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
