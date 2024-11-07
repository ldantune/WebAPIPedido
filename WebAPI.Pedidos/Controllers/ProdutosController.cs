using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Services;

namespace WebAPI.Pedidos.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get(int paginaAtual = 1, int itensPorPagina = 3)
    {
        var produtosDto = await _produtoService.GetProdutos(paginaAtual, itensPorPagina);
        if (produtosDto == null)
        {
            return NotFound("Produtos not found");
        }
        return Ok(produtosDto);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ProdutoDTO produtoDto)
    {
        if (produtoDto == null)
            return BadRequest("Invalid Data");

        await _produtoService.AddProduto(produtoDto);

        return Created(string.Empty, produtoDto);
    }
}
