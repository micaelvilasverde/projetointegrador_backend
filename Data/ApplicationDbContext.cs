using Microsoft.EntityFrameworkCore;
using CosmeticosAPI.Models;

namespace CosmeticosAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<CarrinhoItem> CarrinhoItens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações das entidades
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<ItemPedido>()
                .HasOne(i => i.Pedido)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItemPedido>()
                .HasOne(i => i.Produto)
                .WithMany(p => p.ItensPedido)
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(i => i.Carrinho)
                .WithMany(c => c.Itens)
                .HasForeignKey(i => i.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarrinhoItem>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Pedidos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Carrinho>()
                .HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed de dados iniciais
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categorias
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Perfumaria", Descricao = "Perfumes e fragrâncias" },
                new Categoria { Id = 2, Nome = "Maquiagem", Descricao = "Produtos de maquiagem" },
                new Categoria { Id = 3, Nome = "Hidratante", Descricao = "Cremes e loções hidratantes" }
            );

            // Seed Produtos
            modelBuilder.Entity<Produto>().HasData(
                new Produto 
                { 
                    Id = 1, 
                    Nome = "Perfume Floral", 
                    Descricao = "Perfume com fragrância floral", 
                    Preco = 89.90M, 
                    EstoqueQuantidade = 50, 
                    ImagemUrl = "/imagens/perfume-floral.jpg", 
                    CategoriaId = 1 
                },
                new Produto 
                { 
                    Id = 2, 
                    Nome = "Batom Vermelho", 
                    Descricao = "Batom de longa duração na cor vermelho intenso", 
                    Preco = 35.50M, 
                    EstoqueQuantidade = 100, 
                    ImagemUrl = "/imagens/batom-vermelho.jpg", 
                    CategoriaId = 2 
                },
                new Produto 
                { 
                    Id = 3, 
                    Nome = "Creme Hidratante Facial", 
                    Descricao = "Creme hidratante para pele seca", 
                    Preco = 45.99M, 
                    EstoqueQuantidade = 75, 
                    ImagemUrl = "/imagens/hidratante-facial.jpg", 
                    CategoriaId = 3 
                }
            );

            // Seed Usuário Admin
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nome = "Administrador",
                    Email = "admin@cosmeticos.com",
                    Senha = "Admin@123", // Em produção, utilizaria hash
                    Admin = true
                }
            );
        }
    }
} 