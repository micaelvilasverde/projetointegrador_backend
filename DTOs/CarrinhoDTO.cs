using System.ComponentModel.DataAnnotations;

namespace CosmeticosAPI.DTOs
{
    public class CarrinhoDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; }
        public List<CarrinhoItemDTO>? Itens { get; set; }
        public decimal Total => Itens?.Sum(i => i.Subtotal) ?? 0;
    }

    public class CarrinhoItemDTO
    {
        public int Id { get; set; }
        public int CarrinhoId { get; set; }
        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public string? ProdutoImagemUrl { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal => PrecoUnitario * Quantidade;
    }

    public class AdicionarAoCarrinhoDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
    }

    public class AtualizarCarrinhoItemDTO
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa")]
        public int Quantidade { get; set; }
    }
} 