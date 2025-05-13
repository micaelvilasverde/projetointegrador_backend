using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmeticosAPI.Data;
using CosmeticosAPI.Models;
using CosmeticosAPI.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace CosmeticosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return usuarios.Select(u => new UsuarioDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                Admin = u.Admin
            }).ToList();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Admin = usuario.Admin
            };
        }

        // POST: api/Usuarios/Registrar
        [HttpPost("Registrar")]
        public async Task<ActionResult<UsuarioDTO>> Registrar(UsuarioRegistroDTO registroDto)
        {
            // Verificar se já existe um usuário com o mesmo email
            if (await _context.Usuarios.AnyAsync(u => u.Email == registroDto.Email))
            {
                return BadRequest("Email já está em uso");
            }

            // Criptografar a senha
            string senhaCriptografada = CriptografarSenha(registroDto.Senha);

            var usuario = new Usuario
            {
                Nome = registroDto.Nome,
                Email = registroDto.Email,
                Senha = senhaCriptografada,
                Admin = false // Padrão para usuários comuns
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Criar um carrinho para o usuário
            var carrinho = new Carrinho
            {
                UsuarioId = usuario.Id,
                DataCriacao = DateTime.Now
            };

            _context.Carrinhos.Add(carrinho);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Admin = usuario.Admin
            });
        }

        // POST: api/Usuarios/Login
        [HttpPost("Login")]
        public async Task<ActionResult<UsuarioDTO>> Login(UsuarioLoginDTO loginDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            // Verificar a senha
            string senhaCriptografada = CriptografarSenha(loginDto.Senha);
            if (usuario.Senha != senhaCriptografada)
            {
                return Unauthorized("Email ou senha inválidos");
            }

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Admin = usuario.Admin
            };
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateDTO usuarioDto)
        {
            if (id != usuarioDto.Id)
            {
                return BadRequest();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Verificar se o email já está em uso por outro usuário
            if (usuarioDto.Email != usuario.Email &&
                await _context.Usuarios.AnyAsync(u => u.Email == usuarioDto.Email))
            {
                return BadRequest("Email já está em uso por outro usuário");
            }

            usuario.Nome = usuarioDto.Nome;
            usuario.Email = usuarioDto.Email;

            // Atualizar a senha apenas se uma nova senha for fornecida
            if (!string.IsNullOrEmpty(usuarioDto.NovaSenha))
            {
                usuario.Senha = CriptografarSenha(usuarioDto.NovaSenha);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
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

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Verificar se o usuário tem pedidos
            var temPedidos = await _context.Pedidos.AnyAsync(p => p.UsuarioId == id);
            if (temPedidos)
            {
                return BadRequest("Não é possível excluir um usuário com pedidos registrados");
            }

            // Remover o carrinho do usuário
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == id);

            if (carrinho != null)
            {
                if (carrinho.Itens != null)
                {
                    _context.CarrinhoItens.RemoveRange(carrinho.Itens);
                }
                _context.Carrinhos.Remove(carrinho);
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        private string CriptografarSenha(string senha)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senha));
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
} 