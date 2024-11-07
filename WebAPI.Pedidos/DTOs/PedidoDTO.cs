using WebAPI.Pedidos.Enums;

namespace WebAPI.Pedidos.DTOs;

public class PedidoDTO
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public StatusPedido Status { get; set; }
    public List<PedidoProdutoDTO> Produtos { get; set; } = new List<PedidoProdutoDTO>();

    public string StatusDescricao => Status.ToString();
    public decimal ValorTotal { get; set; }
}
