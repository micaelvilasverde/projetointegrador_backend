using System.ComponentModel.DataAnnotations;

namespace CosmeticosAPI.DTOs
{
    public class CategoriaDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class CategoriaCreateDTO
    {
        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome da categoria deve ter no máximo 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string? Descricao { get; set; }
    }

    public class CategoriaUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatório")]
        [StringLength(50, ErrorMessage = "O nome da categoria deve ter no máximo 50 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres")]
        public string? Descricao { get; set; }
    }
} 