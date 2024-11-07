using System.ComponentModel.DataAnnotations;

namespace WebAPI.Pedidos.DTOs;

public class ProdutoDTO
{
    public int Id { get; set; }


    [Required(ErrorMessage = "Informe o nome do produto")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "Informe o preço do produto")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "Informe o estoque do produto")]
    public int Estoque { get; set; }
}
