﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebAPI.Pedidos.Context;

#nullable disable

namespace WebAPI.Pedidos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241107012648_alteraTabelaPedidos")]
    partial class alteraTabelaPedidos
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("WebAPI.Pedidos.Domain.Entities.Pedido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Pedidos");
                });

            modelBuilder.Entity("WebAPI.Pedidos.Domain.Entities.PedidoProduto", b =>
                {
                    b.Property<int>("PedidoId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ProdutoId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("PrecoUnitario")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantidade")
                        .HasColumnType("INTEGER");

                    b.HasKey("PedidoId", "ProdutoId");

                    b.HasIndex("ProdutoId");

                    b.ToTable("PedidoProdutos");
                });

            modelBuilder.Entity("WebAPI.Pedidos.Domain.Entities.Produto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Estoque")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Preco")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Produtos");
                });

            modelBuilder.Entity("WebAPI.Pedidos.Domain.Entities.PedidoProduto", b =>
                {
                    b.HasOne("WebAPI.Pedidos.Domain.Entities.Pedido", "Pedido")
                        .WithMany("Produtos")
                        .HasForeignKey("PedidoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebAPI.Pedidos.Domain.Entities.Produto", "Produto")
                        .WithMany()
                        .HasForeignKey("ProdutoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pedido");

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("WebAPI.Pedidos.Domain.Entities.Pedido", b =>
                {
                    b.Navigation("Produtos");
                });
#pragma warning restore 612, 618
        }
    }
}
