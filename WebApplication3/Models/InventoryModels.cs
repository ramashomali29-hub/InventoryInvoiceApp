using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryInvoiceApp.Models
{
    
    public class Provider
    {
        [Key]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime SupplyDate { get; set; }
            = DateTime.Today;

        public List<ProviderMaterial> ProviderMaterials
        {
            get;
            set;
        } = new();
    }

    public class Material
    {
        [Key]
        public int MaterialId { get; set; }

        [Required]
        [StringLength(100)]
        public string MaterialName { get; set; } = "";

        public List<ProviderMaterial> ProviderMaterials
        {
            get;
            set;
        } = new();

        public List<CashierRecord> CashierRecords
        {
            get;
            set;
        } = new();
    }

   
    public class ProviderMaterial
    {
        [Key]
        public int SerialNumber { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public Material Material { get; set; } = null!;

        [Required]
        public int ProviderId { get; set; }

        public Provider Provider { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string ProviderName { get; set; } = "";

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
    }

    
    public class CashierRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaterialId { get; set; }

        public Material Material { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }
    }

    public class StockMovement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string MaterialName { get; set; } = "";

        public int QuantityChange { get; set; }
    }
}