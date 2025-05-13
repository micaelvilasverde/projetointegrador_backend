using System.ComponentModel.DataAnnotations;

namespace CosmeticosAPI.Models
{
    public class Carrinho
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Propriedade de navegação para usuário
        public Usuario? Usuario { get; set; }

        // Propriedade de navegação para itens do carrinho
        public ICollection<CarrinhoItem>? Itens { get; set; }
    }
} 