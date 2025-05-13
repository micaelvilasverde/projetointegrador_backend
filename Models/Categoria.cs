using System.ComponentModel.DataAnnotations;

namespace CosmeticosAPI.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome da categoria deve ter no máximo 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string? Descricao { get; set; }

        // Propriedade de navegação para produtos
        public ICollection<Produto>? Produtos { get; set; }
    }
} 