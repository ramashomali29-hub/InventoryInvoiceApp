using InventoryInvoiceApp.Data;
using InventoryInvoiceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryInvoiceApp.Controllers
{
    [Authorize(Roles = "Warehouse,Admin")]
    public class WarehouseController : Controller
    {
        private readonly AppDbContext _context;

        private readonly ILogger<WarehouseController>
            _logger;

        public WarehouseController(
            AppDbContext context,
            ILogger<WarehouseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildViewModelAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddSupply(
            WarehouseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.CurrentStock =
                    await LoadCurrentStockAsync();

                return View("Index", model);
            }

            string providerName =
                model.ProviderName.Trim();

            string materialName =
                model.MaterialName.Trim();

            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();

            try
            {
                var provider =
                    await _context.Providers
                        .FirstOrDefaultAsync(x =>
                            x.ProviderName ==
                            providerName);

                if (provider == null)
                {
                    provider = new Provider
                    {
                        ProviderName = providerName,
                        SupplyDate = model.SupplyDate
                    };

                    _context.Providers.Add(provider);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    provider.SupplyDate =
                        model.SupplyDate;
                }

                var material =
                    await _context.Materials
                        .FirstOrDefaultAsync(x =>
                            x.MaterialName ==
                            materialName);

                if (material == null)
                {
                    material = new Material
                    {
                        MaterialName = materialName
                    };

                    _context.Materials.Add(material);

                    await _context.SaveChangesAsync();
                }

                var stockRecord =
                    await _context.ProviderMaterials
                        .FirstOrDefaultAsync(x =>
                            x.ProviderId ==
                            provider.ProviderId &&
                            x.MaterialId ==
                            material.MaterialId);

                if (stockRecord == null)
                {
                    stockRecord = new ProviderMaterial
                    {
                        ProviderId =
                            provider.ProviderId,

                        MaterialId =
                            material.MaterialId,

                        ProviderName =
                            provider.ProviderName,

                        Quantity =
                            model.Quantity,

                        Price =
                            model.Price
                    };

                    _context.ProviderMaterials.Add(
                        stockRecord);
                }
                else
                {
                    stockRecord.Quantity +=
                        model.Quantity;

                    stockRecord.Price =
                        model.Price;

                    stockRecord.ProviderName =
                        provider.ProviderName;
                }

                
                _context.StockMovements.Add(
                    new StockMovement
                    {
                        MaterialName =
                            material.MaterialName,

                        QuantityChange =
                            model.Quantity
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Warehouse user {User} added {Quantity} units of {Material} from {Provider}.",
                    User.Identity?.Name,
                    model.Quantity,
                    materialName,
                    providerName);

                TempData["Success"] =
                    "Supply saved and stock increased successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception exception)
            {
                await transaction.RollbackAsync();

                _logger.LogError(
                    exception,
                    "Error while adding warehouse supply.");

                ModelState.AddModelError(
                    "",
                    "The supply could not be saved.");

                model.CurrentStock =
                    await LoadCurrentStockAsync();

                return View("Index", model);
            }
        }

        private async Task<WarehouseViewModel>
            BuildViewModelAsync()
        {
            return new WarehouseViewModel
            {
                SupplyDate = DateTime.Today,

                CurrentStock =
                    await LoadCurrentStockAsync()
            };
        }

        private async Task<List<WarehouseItemViewModel>>
            LoadCurrentStockAsync()
        {
            return await _context.ProviderMaterials
                .AsNoTracking()
                .Include(x => x.Material)
                .OrderBy(x => x.Material.MaterialName)
                .Select(x =>
                    new WarehouseItemViewModel
                    {
                        SerialNumber =
                            x.SerialNumber,

                        ProviderName =
                            x.ProviderName,

                        MaterialName =
                            x.Material.MaterialName,

                        Quantity =
                            x.Quantity,

                        Price =
                            x.Price
                    })
                .ToListAsync();
        }
    }
}