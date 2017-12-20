using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ComparaisonRisques.Models;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ComparaisonRisques.Controllers
{
    // Contrôleur destiné à l'accès aux paramètres.
    // Soit une lite brute, soit préformatée pour créer des graphiques.
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

        // Liste des données statistiques brutes
        // Utile dans le cas où le client souhaite traiter les données par lui-même
        // GET: api/parametre
        [HttpGet]
        public IEnumerable<ParametreItem> GetStats()
        {
            _logger.LogInformation("GetStats : Liste des données statistiques.");
            return _context.ParametreItems.ToList();
        }

        // La liste des paramètres disponibles (nom des propriétés) et les détails sur celle-ci (unités, min, max ..)
        // GET: api/parametre/info
        [HttpGet("info")]
        public IActionResult Info()
        {
            _logger.LogInformation("Info : paramètres disponibles (parametre).");
            return new ObjectResult(_context.ParametreInfos.Where(p => p.Groupe == GroupeInfo.Paramètre || p.Groupe == GroupeInfo.PatientEtParamètre).ToList());
        }

        // Retourne un ensemble de point destinés à alimenter un graphique de type "ligne"
        // L'abscisse et l'ordonnée sont choisies parmi les paramètres
        // GET: api/parametre/line_chart/Age/BMI
        [HttpGet("line_chart/{abscisse}/{ordonnee}")]
        public IActionResult LineChart(string abscisse, string ordonnee)
        {
            
            // Vérifie l'existence de l'abscisse
            if (((new ParametreItem().GetType()).GetProperties()).Where(p => p.Name != "Id" && p.Name.ToLower() == abscisse.ToLower()).Count()==0)
            {
                _logger.LogWarning("LineChart : Abscisse {ordonnee} n'éxiste pas.", abscisse);
                return BadRequest("Abscisse : " + abscisse + " n'éxiste pas.");
            }
            // 
            // Vérifie l'existence de l'ordonnée
            if (((new ParametreItem().GetType()).GetProperties()).Where(p => p.Name != "Id" && p.Name.ToLower() == ordonnee.ToLower()).Count() == 0)
            {
                _logger.LogWarning("LineChart : Ordonnee {ordonnee} n'éxiste pas.", ordonnee);
                return BadRequest("Ordonnee : " + ordonnee + " n'éxiste pas.");
            }

            // À partie de la liste des paramètres
            // Range dans l'ordre croissant la propiété choisie pour l'abscisse
            // Groupe le résultat par la propiété choisie pour l'abscisse
            // La sortie est un tableau avec en première entrée la propiété choisie pour l'abscisse
            // et en seconde entrée une moyenne de la propiété choisie pour l'ordonnée
            List<double[]> listePoints = _context.ParametreItems
                .OrderBy(p => p.GetType().GetProperty(abscisse, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(p))
                .GroupBy(p => p.GetType().GetProperty(abscisse, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(p))
                .Select(g => (new[] { Math.Round(double.Parse(g.Key.ToString()),2),
                        Math.Round(g.Average(s => double.Parse(s.GetType().GetProperty(ordonnee,BindingFlags.IgnoreCase |  BindingFlags.Public | BindingFlags.Instance).GetValue(s).ToString())),2) })).ToList();

            _logger.LogInformation("LineChart : points demandés abscisse: {abscisse}, ordonnée: {ordonnee}.", abscisse, ordonnee);

            return new ObjectResult(listePoints);

        }

        // Retourne un ensemble de point destinés à alimenter un graphique de type "nuage de point"
        // L'abscisse et l'ordonnée sont choisies parmi les paramètres
        // GET: api/parametre/scatter_chart/Age/BMI
        [HttpGet("scatter_chart/{abscisse}/{ordonnee}")]
        public IActionResult ScatterChart(string abscisse, string ordonnee)
        {
            // Vérifie l'existence de l'abscisse
            if (((new ParametreItem().GetType()).GetProperties()).Where(p => p.Name != "Id" && p.Name.ToLower() == abscisse.ToLower()).Count() == 0)
            {
                _logger.LogWarning("ScatterChart : Abscisse {ordonnee} n'éxiste pas.", abscisse);
                return BadRequest("Abscisse : " + abscisse + " n'éxiste pas.");
            }

            // Vérifie l'existence de l'ordonnée
            if (((new ParametreItem().GetType()).GetProperties()).Where(p => p.Name != "Id" && p.Name.ToLower() == ordonnee.ToLower()).Count() == 0)
            {
                _logger.LogWarning("ScatterChart : Ordonnee {ordonnee} n'éxiste pas.", ordonnee);
                return BadRequest("Ordonnee : " + ordonnee + " n'éxiste pas.");
            }

            // À partie de la liste des paramètres
            // La sortie est un tableau avec en première entrée la propiété choisie pour l'abscisse
            // et en seconde entrée la propiété choisie pour l'ordonnée
            List<double[]> listePoints = _context.ParametreItems
                .Select(g => (new[] { Math.Round(double.Parse(g.GetType().GetProperty(abscisse, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(g).ToString()),2),
                                        Math.Round(double.Parse(g.GetType().GetProperty(ordonnee, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).GetValue(g).ToString()),2) })).ToList();

            _logger.LogInformation("ScatterChart : points demandés abscisse: {abscisse}, ordonnée: {ordonnee}.", abscisse, ordonnee);

            return new ObjectResult(listePoints);

        }
    }
}
