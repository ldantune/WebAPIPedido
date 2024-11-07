using WebAPI.Pedidos.Enums;

namespace WebAPI.Pedidos.Entities;

public class Pedido
{
    public int Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public StatusPedido Status { get; set; } = StatusPedido.Aberto;
    public List<PedidoProduto> Produtos { get; set; } = new List<PedidoProduto>();

}
