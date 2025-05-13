using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmeticosAPI.Data;
using CosmeticosAPI.Models;
using CosmeticosAPI.DTOs;

namespace CosmeticosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarrinhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Carrinho/Usuario/5
        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<CarrinhoDTO>> GetCarrinho(int usuarioId)
        {
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null)
            {
                // Se o usuário não tem carrinho, criar um novo
                var usuario = await _context.Usuarios.FindAsync(usuarioId);
                if (usuario == null)
                {
                    return NotFound("Usuário não encontrado");
                }

                carrinho = new Carrinho
                {
                    UsuarioId = usuarioId,
                    DataCriacao = DateTime.Now,
                    Itens = new List<CarrinhoItem>()
                };

                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            return new CarrinhoDTO
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                DataCriacao = carrinho.DataCriacao,
                Itens = carrinho.Itens?.Select(i => new CarrinhoItemDTO
                {
                    Id = i.Id,
                    CarrinhoId = i.CarrinhoId,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto?.Nome,
                    ProdutoImagemUrl = i.Produto?.ImagemUrl,
                    PrecoUnitario = i.PrecoUnitario,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }

        // POST: api/Carrinho/AdicionarItem
        [HttpPost("AdicionarItem")]
        public async Task<ActionResult<CarrinhoDTO>> AdicionarItem(AdicionarAoCarrinhoDTO itemDto)
        {
            // Verificar se o usuário existe
            var usuario = await _context.Usuarios.FindAsync(itemDto.UsuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Verificar se o produto existe
            var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            // Verificar se o estoque é suficiente
            if (produto.EstoqueQuantidade < itemDto.Quantidade)
            {
                return BadRequest("Quantidade solicitada não disponível em estoque");
            }

            // Buscar ou criar um carrinho para o usuário
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == itemDto.UsuarioId);

            if (carrinho == null)
            {
                carrinho = new Carrinho
                {
                    UsuarioId = itemDto.UsuarioId,
                    DataCriacao = DateTime.Now,
                    Itens = new List<CarrinhoItem>()
                };
                _context.Carrinhos.Add(carrinho);
                await _context.SaveChangesAsync();
            }

            // Verificar se o produto já está no carrinho
            var carrinhoItem = carrinho.Itens?.FirstOrDefault(i => i.ProdutoId == itemDto.ProdutoId);

            if (carrinhoItem != null)
            {
                // Atualizar a quantidade
                carrinhoItem.Quantidade += itemDto.Quantidade;
                carrinhoItem.PrecoUnitario = produto.Preco; // Atualizar o preço caso tenha mudado
            }
            else
            {
                // Adicionar novo item ao carrinho
                carrinhoItem = new CarrinhoItem
                {
                    CarrinhoId = carrinho.Id,
                    ProdutoId = itemDto.ProdutoId,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = produto.Preco
                };
                if (carrinho.Itens == null)
                {
                    carrinho.Itens = new List<CarrinhoItem>();
                }
                carrinho.Itens.Add(carrinhoItem);
            }

            await _context.SaveChangesAsync();

            // Carregar os dados do produto para retornar o DTO completo
            await _context.Entry(carrinhoItem).Reference(i => i.Produto).LoadAsync();

            // Recarregar o carrinho completo
            carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.Id == carrinho.Id);

            return new CarrinhoDTO
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                DataCriacao = carrinho.DataCriacao,
                Itens = carrinho.Itens?.Select(i => new CarrinhoItemDTO
                {
                    Id = i.Id,
                    CarrinhoId = i.CarrinhoId,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto?.Nome,
                    ProdutoImagemUrl = i.Produto?.ImagemUrl,
                    PrecoUnitario = i.PrecoUnitario,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }

        // PUT: api/Carrinho/AtualizarQuantidade
        [HttpPut("AtualizarQuantidade")]
        public async Task<ActionResult<CarrinhoDTO>> AtualizarQuantidade(AtualizarCarrinhoItemDTO itemDto)
        {
            var carrinhoItem = await _context.CarrinhoItens
                .Include(i => i.Carrinho)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(i => i.Id == itemDto.ItemId);

            if (carrinhoItem == null)
            {
                return NotFound("Item não encontrado no carrinho");
            }

            if (itemDto.Quantidade <= 0)
            {
                // Remover o item se a quantidade for zero ou negativa
                _context.CarrinhoItens.Remove(carrinhoItem);
            }
            else
            {
                // Verificar se tem estoque suficiente
                if (carrinhoItem.Produto.EstoqueQuantidade < itemDto.Quantidade)
                {
                    return BadRequest("Quantidade solicitada não disponível em estoque");
                }

                // Atualizar a quantidade
                carrinhoItem.Quantidade = itemDto.Quantidade;
            }

            await _context.SaveChangesAsync();

            // Recarregar o carrinho completo
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.Id == carrinhoItem.CarrinhoId);

            return new CarrinhoDTO
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                DataCriacao = carrinho.DataCriacao,
                Itens = carrinho.Itens?.Select(i => new CarrinhoItemDTO
                {
                    Id = i.Id,
                    CarrinhoId = i.CarrinhoId,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto?.Nome,
                    ProdutoImagemUrl = i.Produto?.ImagemUrl,
                    PrecoUnitario = i.PrecoUnitario,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }

        // DELETE: api/Carrinho/RemoverItem/5
        [HttpDelete("RemoverItem/{itemId}")]
        public async Task<ActionResult<CarrinhoDTO>> RemoverItem(int itemId)
        {
            var carrinhoItem = await _context.CarrinhoItens
                .Include(i => i.Carrinho)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (carrinhoItem == null)
            {
                return NotFound("Item não encontrado no carrinho");
            }

            var carrinhoId = carrinhoItem.CarrinhoId;

            _context.CarrinhoItens.Remove(carrinhoItem);
            await _context.SaveChangesAsync();

            // Recarregar o carrinho completo
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.Id == carrinhoId);

            return new CarrinhoDTO
            {
                Id = carrinho.Id,
                UsuarioId = carrinho.UsuarioId,
                DataCriacao = carrinho.DataCriacao,
                Itens = carrinho.Itens?.Select(i => new CarrinhoItemDTO
                {
                    Id = i.Id,
                    CarrinhoId = i.CarrinhoId,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto?.Nome,
                    ProdutoImagemUrl = i.Produto?.ImagemUrl,
                    PrecoUnitario = i.PrecoUnitario,
                    Quantidade = i.Quantidade
                }).ToList()
            };
        }

        // POST: api/Carrinho/LimparCarrinho
        [HttpPost("LimparCarrinho/{usuarioId}")]
        public async Task<ActionResult> LimparCarrinho(int usuarioId)
        {
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null || carrinho.Itens == null || !carrinho.Itens.Any())
            {
                return Ok("Carrinho já está vazio");
            }

            _context.CarrinhoItens.RemoveRange(carrinho.Itens);
            await _context.SaveChangesAsync();

            return Ok("Carrinho esvaziado com sucesso");
        }

        // POST: api/Carrinho/FinalizarPedido
        [HttpPost("FinalizarPedido/{usuarioId}")]
        public async Task<ActionResult<PedidoDTO>> FinalizarPedido(int usuarioId)
        {
            // Verificar se o usuário existe
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            // Buscar o carrinho do usuário
            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null || carrinho.Itens == null || !carrinho.Itens.Any())
            {
                return BadRequest("Carrinho está vazio");
            }

            // Verificar estoque para todos os itens
            foreach (var item in carrinho.Itens)
            {
                if (item.Produto.EstoqueQuantidade < item.Quantidade)
                {
                    return BadRequest($"Produto '{item.Produto.Nome}' não tem estoque suficiente");
                }
            }

            // Criar o pedido
            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                ItensPedido = new List<ItemPedido>(),
                Total = carrinho.Itens.Sum(i => i.PrecoUnitario * i.Quantidade)
            };

            foreach (var item in carrinho.Itens)
            {
                // Adicionar o item ao pedido
                var itemPedido = new ItemPedido
                {
                    ProdutoId = item.ProdutoId,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.PrecoUnitario,
                    Subtotal = item.PrecoUnitario * item.Quantidade
                };

                pedido.ItensPedido.Add(itemPedido);

                // Atualizar o estoque
                item.Produto.EstoqueQuantidade -= item.Quantidade;
            }

            _context.Pedidos.Add(pedido);

            // Limpar o carrinho
            _context.CarrinhoItens.RemoveRange(carrinho.Itens);

            await _context.SaveChangesAsync();

            // Recarregar o pedido com todos os relacionamentos
            pedido = await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.ItensPedido)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == pedido.Id);

            return new PedidoDTO
            {
                Id = pedido.Id,
                DataPedido = pedido.DataPedido,
                Status = pedido.Status,
                Total = pedido.Total,
                UsuarioId = pedido.UsuarioId,
                UsuarioNome = pedido.Usuario?.Nome,
                ItensPedido = pedido.ItensPedido?.Select(i => new ItemPedidoDTO
                {
                    Id = i.Id,
                    PedidoId = i.PedidoId,
                    ProdutoId = i.ProdutoId,
                    ProdutoNome = i.Produto?.Nome,
                    ProdutoImagemUrl = i.Produto?.ImagemUrl,
                    PrecoUnitario = i.PrecoUnitario,
                    Quantidade = i.Quantidade,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }
    }
} 