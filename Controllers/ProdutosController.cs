using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmeticosAPI.Data;
using CosmeticosAPI.Models;
using CosmeticosAPI.DTOs;

namespace CosmeticosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProdutosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutos()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .ToListAsync();

            return produtos.Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                EstoqueQuantidade = p.EstoqueQuantidade,
                ImagemUrl = p.ImagemUrl,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome
            }).ToList();
        }

        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDTO>> GetProduto(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
            {
                return NotFound();
            }

            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                EstoqueQuantidade = produto.EstoqueQuantidade,
                ImagemUrl = produto.ImagemUrl,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome
            };
        }

        // GET: api/Produtos/Categoria/5
        [HttpGet("Categoria/{categoriaId}")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPorCategoria(int categoriaId)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.CategoriaId == categoriaId)
                .ToListAsync();

            return produtos.Select(p => new ProdutoDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                EstoqueQuantidade = p.EstoqueQuantidade,
                ImagemUrl = p.ImagemUrl,
                CategoriaId = p.CategoriaId,
                CategoriaNome = p.Categoria?.Nome
            }).ToList();
        }

        // POST: api/Produtos
        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> PostProduto(ProdutoCreateDTO produtoDto)
        {
            // Verificar se a categoria existe
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == produtoDto.CategoriaId);
            if (!categoriaExiste)
            {
                return BadRequest("Categoria não encontrada");
            }

            var produto = new Produto
            {
                Nome = produtoDto.Nome,
                Descricao = produtoDto.Descricao,
                Preco = produtoDto.Preco,
                EstoqueQuantidade = produtoDto.EstoqueQuantidade,
                ImagemUrl = produtoDto.ImagemUrl,
                CategoriaId = produtoDto.CategoriaId
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            // Carregar a categoria para retornar o nome
            await _context.Entry(produto).Reference(p => p.Categoria).LoadAsync();

            var produtoCriado = new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                EstoqueQuantidade = produto.EstoqueQuantidade,
                ImagemUrl = produto.ImagemUrl,
                CategoriaId = produto.CategoriaId,
                CategoriaNome = produto.Categoria?.Nome
            };

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produtoCriado);
        }

        // PUT: api/Produtos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, ProdutoUpdateDTO produtoDto)
        {
            if (id != produtoDto.Id)
            {
                return BadRequest();
            }

            // Verificar se o produto existe
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            // Verificar se a categoria existe
            var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == produtoDto.CategoriaId);
            if (!categoriaExiste)
            {
                return BadRequest("Categoria não encontrada");
            }

            // Atualizar os campos do produto
            produto.Nome = produtoDto.Nome;
            produto.Descricao = produtoDto.Descricao;
            produto.Preco = produtoDto.Preco;
            produto.EstoqueQuantidade = produtoDto.EstoqueQuantidade;
            produto.ImagemUrl = produtoDto.ImagemUrl;
            produto.CategoriaId = produtoDto.CategoriaId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Produtos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }

            // Verificar se o produto está em algum pedido
            var temPedidos = await _context.ItensPedido.AnyAsync(i => i.ProdutoId == id);
            if (temPedidos)
            {
                return BadRequest("Não é possível excluir um produto que possui pedidos associados.");
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }
    }
} 