using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ComparaisonRisques.Models;
using Microsoft.Extensions.Logging;

namespace ComparaisonRisques.Controllers
{
    [Produces("application/json")]
    [Route("api/parametre")]
    public class ParametreController : Controller
    {

        private readonly MyContext _context;
        private readonly ILogger _logger;

        public ParametreController(ILogger<ParametreController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/parametre
        [HttpGet]
        public IEnumerable<ParametreItem> Get()
        {
            return _context.ParametreItems.ToList();
        }
    }
}
