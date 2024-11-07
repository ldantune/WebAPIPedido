using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebAPI.Pedidos.Context;
using WebAPI.Pedidos.Entities;
using WebAPI.Pedidos.Repositories;

namespace WebAPI.Tests.Repositories;
public class ProdutoRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly ProdutoRepository _produtoRepository;

    public ProdutoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _produtoRepository = new ProdutoRepository(_context);
    }

    [Fact]
    public async Task GetAll_Should_Return_Produtos_With_Pagination()
    {
        var produto1 = new Produto { Id = 1, Nome = "Produto 1", Preco = 10 };
        var produto2 = new Produto { Id = 2, Nome = "Produto 2", Preco = 20 };
        var produto3 = new Produto { Id = 3, Nome = "Produto 3", Preco = 30 };
        var produto4 = new Produto { Id = 4, Nome = "Produto 4", Preco = 40 };

        await _context.Produtos.AddRangeAsync(produto1, produto2, produto3, produto4);
        await _context.SaveChangesAsync();

        var result = await _produtoRepository.GetAll(1, 2); 


        result.Should().HaveCount(2);  
        result.Should().Contain(p => p.Id == 1);
        result.Should().Contain(p => p.Id == 2);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Produto()
    {
        var produto = new Produto { Id = 1, Nome = "Produto 1", Preco = 10 };
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();

        var result = await _produtoRepository.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Nome.Should().Be("Produto 1");
        result.Preco.Should().Be(10);
    }

    [Fact]
    public async Task Create_Should_Add_Produto()
    {
        var produto = new Produto { Nome = "Produto 1", Preco = 10 };

        var result = await _produtoRepository.Create(produto);

        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0); 
        result.Nome.Should().Be("Produto 1");
        result.Preco.Should().Be(10);

        var produtoNoDb = await _context.Produtos.FindAsync(result.Id);
        produtoNoDb.Should().NotBeNull();
        produtoNoDb.Nome.Should().Be("Produto 1");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Produto()
    {
        var produto = new Produto { Id = 1, Nome = "Produto 1", Preco = 10 };
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();

        produto.Nome = "Produto Atualizado";
        produto.Preco = 15;

        await _produtoRepository.UpdateAsync(produto);

        var updatedProduto = await _context.Produtos.FindAsync(1);
        updatedProduto.Should().NotBeNull();
        updatedProduto.Nome.Should().Be("Produto Atualizado");
        updatedProduto.Preco.Should().Be(15);
    }

}
