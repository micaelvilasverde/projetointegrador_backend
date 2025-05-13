using System.ComponentModel.DataAnnotations;

namespace CosmeticosAPI.DTOs
{
    public class ProdutoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int EstoqueQuantidade { get; set; }
        public string? ImagemUrl { get; set; }
        public int CategoriaId { get; set; }
        public string? CategoriaNome { get; set; }
    }

    public class ProdutoCreateDTO
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa")]
        public int EstoqueQuantidade { get; set; }

        public string? ImagemUrl { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoriaId { get; set; }
    }

    public class ProdutoUpdateDTO
    {
        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do produto é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do produto deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A quantidade em estoque é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa")]
        public int EstoqueQuantidade { get; set; }

        public string? ImagemUrl { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoriaId { get; set; }
    }
} 