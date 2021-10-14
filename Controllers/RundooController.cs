using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RundooApi.Models;
using RundooApi.Services;
using System;

namespace RundooApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class RundooController : Controller
    {
        private readonly DocDBService _docDBService;
        private readonly TablesService _tablesService;

        public RundooController(DocDBService docDBService, TablesService tablesService)
        {
            this._docDBService = docDBService;
        }

        [HttpGet]
        [Route("/GetTransaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransaction(TransactionQueryType transactionQueryType, string id)
        {

            return this.Ok(await this._docDBService.GetTransactionAsync(transactionQueryType, id).ConfigureAwait(false));
        }

        [HttpPost]
        [Route("/CreateTransaction")]
        public async Task<ActionResult<IEnumerable<Transaction>>> CreateTransaction(Transaction transaction)
        {

            return this.Ok(await this._docDBService.AddTransactionAsync(transaction).ConfigureAwait(false));
        }
    }
}
