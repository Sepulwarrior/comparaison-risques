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

        // GET: api/parametre
        [HttpGet("graph")]
        public IActionResult Graph()
        {
            return new ObjectResult(((new ParametreItem().GetType()).GetProperties()).Where(p => p.Name != "Id").Select(p=>p.Name));
        }

        // GET: api/parametre/graph/Age/BMI
        [HttpGet("graph/{abscisse}/{ordonnee}")]
        public IActionResult Graph(string abscisse, string ordonnee)
        {

            if (new ParametreItem().GetType().GetProperty(abscisse) == null)
            {
                return BadRequest("Abscisse : " + abscisse + " n'éxiste pas.");
            }

            if (new ParametreItem().GetType().GetProperty(ordonnee) == null)
            {
                return BadRequest("Ordonnee : " + ordonnee + " n'éxiste pas.");
            }

            var retour = _context.ParametreItems
                .OrderBy(p => p.GetType().GetProperty(abscisse).GetValue(p))
                .GroupBy(p => p.GetType().GetProperty(abscisse).GetValue(p))
                .Select(g => (new[] { double.Parse(g.Key.ToString()), g.Average(s => double.Parse(s.GetType().GetProperty(ordonnee).GetValue(s).ToString())) }));

            return new ObjectResult(retour);

        }

        // GET: api/parametre/scatter_chart/Age/BMI
        [HttpGet("scatter_chart/{abscisse}/{ordonnee}")]
        public IActionResult ScatterChart(string abscisse, string ordonnee)
        {

            if (new ParametreItem().GetType().GetProperty(abscisse) == null)
            {
                return BadRequest("Abscisse : " + abscisse + " n'éxiste pas.");
            }

            if (new ParametreItem().GetType().GetProperty(ordonnee) == null)
            {
                return BadRequest("Ordonnee : " + ordonnee + " n'éxiste pas.");
            }

            var retour = _context.ParametreItems
                .Select(g => (new[] { double.Parse(g.GetType().GetProperty(abscisse).GetValue(g).ToString()), double.Parse(g.GetType().GetProperty(ordonnee).GetValue(g).ToString()) }));

            return new ObjectResult(retour);

        }
    }
}
