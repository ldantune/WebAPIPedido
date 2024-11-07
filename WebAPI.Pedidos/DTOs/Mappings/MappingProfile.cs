using AutoMapper;
using WebAPI.Pedidos.Entities;

namespace WebAPI.Pedidos.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<Produto, ProdutoDTO>();
        CreateMap<ProdutoDTO, Produto>();
        CreateMap<Pedido, PedidoDTO>()
           .ForMember(dest => dest.Produtos, opt => opt.MapFrom(src => src.Produtos)); // Exemplo de mapeamento de coleção de produtos

        CreateMap<PedidoProduto, PedidoProdutoDTO>();
        CreateMap<PedidoProdutoDTO, PedidoProduto>();
    }
}
