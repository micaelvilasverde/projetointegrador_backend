using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticosAPI.Models
{
    public class CarrinhoItem
    {
        public int Id { get; set; }

        [Required]
        public int CarrinhoId { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; }

        // Propriedade de navegação para produto
        public Produto? Produto { get; set; }

        // Propriedade de navegação para carrinho
        public Carrinho? Carrinho { get; set; }
    }
} 