using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using RundooApi.Models;
using RundooApi.Services;
using Azure.Data.Tables;

namespace RundooApi.Controllers
{
    [ApiController]
    public class RundooController : Controller
    {
        private readonly DocDBService _docDBService;
        private readonly TablesService _tablesService;

        public RundooController(DocDBService docDBService, TablesService tablesService)
        {
            this._docDBService = docDBService;
            this._tablesService = tablesService;
        }

        [HttpPost]
        [Route("api/[controller]/CreateSupplier")]
        public async Task<ActionResult> CreateSupplier(string supplierId, string[] locationIds)
        {
            foreach (var locationId in locationIds)
            {
                var entry = new LocationEntry(supplierId, locationId);
                await this._tablesService.InsertTableEntity(entry.ConvertToTableEntity()).ConfigureAwait(false);
            }

            return this.Ok();
        }

        [HttpGet]
        [Route("api/[controller]/GetSupplierLocations")]
        public ActionResult<IEnumerable<ProductInventory>> GetSupplier(string supplierId)
        {
            return this.Ok(this._tablesService.QuerySupplierLocationInfo(supplierId));
        }

        [HttpPost]
        [Route("api/[controller]/CreateProductInfo")]
        public async Task<ActionResult<IEnumerable<ProductEntry>>> CreateProductInfo(ProductInfo info)
        {
            var entry = new ProductEntry(info);
            var tableResponse = await this._tablesService.InsertTableEntity(entry.ConvertToTableEntity()).ConfigureAwait(false);

            return this.Ok();
        }

        [HttpGet]
        [Route("api/[controller]/GetInventoryByLocation")]
        public ActionResult<IEnumerable<ProductInventory>> GetInventoryByLocation(string supplierId, string locationId)
        {
            return this.Ok(this._tablesService.QueryInventoryByLocation(supplierId, locationId));
        }

        [HttpGet]
        [Route("api/[controller]/GetProductCountAtLocation")]
        public ActionResult<IEnumerable<ProductInventory>> GetProductCountAtLocation(string supplierId, string locationId, string productId)
        {
            return this.Ok(this._tablesService.QueryProductCountAtLocation(supplierId, locationId, productId));
        }

        [HttpPost]
        [Route("api/[controller]/CreateCustomerInfo")]
        public async Task<ActionResult> CreateCustomerInfo(string supplierId, string customerId)
        {
            var entry = new CustomerEntry(supplierId, customerId);
            await this._tablesService.InsertTableEntity(entry.ConvertToTableEntity()).ConfigureAwait(false);

            return this.Ok();
        }

        [HttpGet]
        [Route("api/[controller]/GetCustomerInfo")]
        public async Task<ActionResult<IEnumerable<string>>> GetCustomerInfo(string supplierId, string customerId)
        {
            var tableEntity = await this._tablesService.GetTableEntity(string.Format(CustomerEntry.TableEntityKeyFormat, supplierId), customerId).ConfigureAwait(false);
            return this.Ok(tableEntity.Value.RowKey);
        }

        [HttpGet]
        [Route("api/[controller]/GetSupplierCustomerInfo")]
        public ActionResult<IEnumerable<string>> QuerySupplierCustomerInfo(string supplierId)
        {
            return this.Ok(this._tablesService.QuerySupplierCustomerInfo(supplierId));
        }

        [HttpGet]
        [Route("api/[controller]/GetTransaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransaction(TransactionQueryType transactionQueryType, string id)
        {
            return this.Ok(await this._docDBService.GetTransactionAsync(transactionQueryType, id).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("api/[controller]/CreateTransaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> CreateTransaction(Transaction transaction)
        {
            var getResponse = await this._tablesService.GetTableEntity(string.Format(ProductEntry.TableEntityKeyFormat, transaction.SupplierId, transaction.LocationId), transaction.ProductId).ConfigureAwait(false);
            TableEntity entity = getResponse.Value;

            entity["Quantity"] = (int)entity["Quantity"]-(int)transaction.Quantity;

            await this._tablesService.UpsertTableEntity(entity);

            return this.Ok(await this._docDBService.AddTransactionAsync(transaction).ConfigureAwait(false));
        }
    }
}
