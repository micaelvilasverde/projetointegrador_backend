using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmeticosAPI.Data;
using CosmeticosAPI.Models;
using CosmeticosAPI.DTOs;

namespace CosmeticosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias()
        {
            var categorias = await _context.Categorias.ToListAsync();
            
            return categorias.Select(c => new CategoriaDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao
            }).ToList();
        }

        // GET: api/Categorias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao
            };
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> PostCategoria(CategoriaCreateDTO categoriaDto)
        {
            var categoria = new Categoria
            {
                Nome = categoriaDto.Nome,
                Descricao = categoriaDto.Descricao
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            var createdCategoria = new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao
            };

            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.Id }, createdCategoria);
        }

        // PUT: api/Categorias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategoria(int id, CategoriaUpdateDTO categoriaDto)
        {
            if (id != categoriaDto.Id)
            {
                return BadRequest();
            }

            var categoria = await _context.Categorias.FindAsync(id);
            
            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Nome = categoriaDto.Nome;
            categoria.Descricao = categoriaDto.Descricao;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaExists(id))
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

        // DELETE: api/Categorias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }

            // Verificar se existem produtos associados à categoria
            var temProdutos = await _context.Produtos.AnyAsync(p => p.CategoriaId == id);
            if (temProdutos)
            {
                return BadRequest("Não é possível excluir uma categoria que possui produtos associados.");
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.Id == id);
        }
    }
} 