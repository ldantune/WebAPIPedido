using AutoMapper;
using WebAPI.Pedidos.DTOs;
using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Repositories;

namespace WebAPI.Pedidos.Services;

public class ProdutoService : IProdutoService
{
    private IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public ProdutoService(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProdutoDTO>> GetProdutos(int paginaAtual, int itensPorPagina)
    {
        var produtosEntity = await _produtoRepository.GetAll(paginaAtual, itensPorPagina);
        return _mapper.Map<IEnumerable<ProdutoDTO>>(produtosEntity);
    }

    public async Task AddProduto(ProdutoDTO produtoDTO)
    {
        var produtoEntity = _mapper.Map<Produto>(produtoDTO);
        await _produtoRepository.Create(produtoEntity);
        produtoDTO.Id = produtoEntity.Id;
    }
}
