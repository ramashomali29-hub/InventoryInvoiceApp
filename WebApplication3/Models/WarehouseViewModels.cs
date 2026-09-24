using System.ComponentModel.DataAnnotations;

namespace InventoryInvoiceApp.Models
{
    public class WarehouseViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Provider Name")]
        public string ProviderName { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Supply Date")]
        public DateTime SupplyDate { get; set; }
            = DateTime.Today;

        [Required]
        [StringLength(100)]
        [Display(Name = "Material Name")]
        public string MaterialName { get; set; } = "";

        [Range(1, 100000)]
        public int Quantity { get; set; } = 1;

        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        public List<WarehouseItemViewModel> CurrentStock
        {
            get;
            set;
        } = new();
    }

    public class WarehouseItemViewModel
    {
        public int SerialNumber { get; set; }

        public string ProviderName { get; set; } = "";

        public string MaterialName { get; set; } = "";

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}