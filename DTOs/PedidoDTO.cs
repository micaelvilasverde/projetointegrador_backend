using System.ComponentModel.DataAnnotations;

namespace CosmeticosAPI.DTOs
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public DateTime DataPedido { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public int UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public List<ItemPedidoDTO>? ItensPedido { get; set; }
        public string ProdutoImagemUrl { get; set; }
    }

    public class PedidoCreateDTO
    {
        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public List<ItemPedidoCreateDTO> ItensPedido { get; set; } = new List<ItemPedidoCreateDTO>();
    }

    public class PedidoUpdateStatusDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }

    public class ItemPedidoDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int ProdutoId { get; set; }
        public string? ProdutoNome { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal { get; set; }
        public string? ProdutoImagemUrl { get; internal set; }
    }

    public class ItemPedidoCreateDTO
    {
        [Required]
        public int ProdutoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
    }
} 