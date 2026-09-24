using System.ComponentModel.DataAnnotations;

namespace InventoryInvoiceApp.Models
{
    public class CashierViewModel
    {
        public List<CashierItemViewModel> Items { get; set; }
            = new();

        public decimal GrandTotal =>
            Items.Sum(x => x.SellQuantity * x.Price);
    }

    public class CashierItemViewModel
    {
        public int MaterialId { get; set; }

        public string MaterialName { get; set; } = "";

        public int AvailableQuantity { get; set; }

        public decimal Price { get; set; }

        [Range(0, 100000)]
        public int SellQuantity { get; set; }
    }
}