using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CosmeticosAPI.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioId { get; set; }

        // Propriedade de navegação para usuário
        public Usuario? Usuario { get; set; }

        // Status do pedido (Pendente, Pago, Enviado, Entregue, Cancelado)
        [Required]
        public string Status { get; set; } = "Pendente";

        // Propriedade de navegação para itens do pedido
        public ICollection<ItemPedido>? ItensPedido { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
    }
} 