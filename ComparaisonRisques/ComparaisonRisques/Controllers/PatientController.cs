using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ComparaisonRisques.Models;
using Microsoft.Extensions.Logging;

using Microsoft.EntityFrameworkCore;

namespace ComparaisonRisques.Controllers
{
    [Produces("application/json")]
    [Route("api/patient")]
    public class PatientController : Controller
    {

        private readonly MyContext _context;
        private readonly ILogger _logger;

        public PatientController(ILogger<PatientController> logger,MyContext context)
        {
            _logger = logger;
            _context = context;  
        }

        // Le C du CRUD : renvoie l'entité crée
        // POST: api/patient
        [HttpPost]
        public IActionResult Create([FromBody]PatientItem patientItem)
        {
            if (patientItem == null) {
                _logger.LogWarning("Create : Patient vide ou incomplet.");
                return BadRequest("Patient vide ou incomplet.");
            }

            // l'auto-incrément fonctionne lorsque la base de donnée est vide.
            // avec l'insertion des données de tests, il tente de créer la première entité avec l'Id 1 ( les 500 premiers existent )
            // j'ai donc contourné l'auto-incrément en faisant max+1 ci dessous
            if (patientItem.Id == 0)
            {
                patientItem.Id = _context.PatientItems.Max(t => t.Id) + 1;
            }
            else
            {
                _logger.LogWarning("Create : Id doit être égal à 0.");
                return BadRequest("Id doit être égal à 0.");
            }

            // Vérifie la validité des données
            TryValidateModel(patientItem);
            if (!ModelState.IsValid)
            {
                // Crée la liste des erreurs provenant de la validation des données.
                var errorList = (from item in ModelState where item.Value.Errors.Any() select item.Value.Errors[0].ErrorMessage).ToList();

                _logger.LogWarning("Create : Données du patient invalides.");

                // Si la validation échoue, retourne 400 + liste des erreurs (JSON)
                return BadRequest(errorList);
            }

            // Ajout du patient et du paramètre associé 
            _context.PatientItems.Add(patientItem);
            _context.ParametreItems.Add(new ParametreItem(patientItem));
            _context.SaveChanges();

            _logger.LogInformation("Create : Patient créé : {id} ", patientItem.Id);

            return CreatedAtRoute("Read", new { id = patientItem.Id }, patientItem);
        }

        // Le R du CRUD : Renvoie toute les entités
        // GET: api/patient
        [HttpGet]
        public IEnumerable<PatientItem> Read(string search="", int limit=0)
        {
            var p = _context.ParametreInfos;

            var patientItems = _context.PatientItems
                .Include("Admin")
                .Include("Biometrie")
                .Include("Const_biologique")
                .Include("Parametres")
                .Include("Assuetudes")
                .ToList();

            // Filtre sur le Nom
            if (search != "")
            {
                patientItems = patientItems.Where(t => t.Admin.Nom.ToLower().StartsWith(search.ToLower())).ToList();
            }

            // Limite ne nombre d'entrées
            if (limit != 0)
            {
                patientItems = patientItems.Take(limit).ToList();
            }
            
            _logger.LogInformation("Read : Liste des patients ({count})" + (search == null ? "" : " recherche : " + search) + (limit == 0 ? "" : " limite : " + limit)+".", patientItems.Count());
            return patientItems;
        }

        // Le R du CRUD : Avec ID, renvoie l'entité demandée
        // GET: api/patient/5
        [HttpGet("{id}", Name = "Read")]
        public IActionResult Read(int id)
        {
            PatientItem patientItem = _context.PatientItems
                            .Include("Admin")
                            .Include("Biometrie")
                            .Include("Const_biologique")
                            .Include("Parametres")
                            .Include("Assuetudes")
                            .FirstOrDefault(t => t.Id == id);

            if (patientItem == null) {
                _logger.LogWarning("Read : Patient {id} non trouvé.", id);
                return NotFound("Patient "+id+" non trouvé.");
            }

            _logger.LogInformation("Read : Lecture patient {id}.", patientItem.Id);
            return new ObjectResult(patientItem);
        }
        
        // Le U de CRUD
        // PUT: api/patient/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]PatientItem patientItem)
        {

            if (patientItem == null) {
                _logger.LogWarning("Update : Patient vide ou incomplet.");
                return BadRequest("Patient vide ou incomplet.");
            }

            if (patientItem.Id != id) {
                _logger.LogWarning("Update : L'Id patient ne correspond pas.");
                return BadRequest("L'Id patient ne correspond pas.");
            }
                        
            int patientExists = _context.PatientItems.Count(t => t.Id == id);
            if (patientExists==0)
            {
                _logger.LogWarning("Update : Patient {id} non trouvé.", id);
                return NotFound("Patient "+id+" non trouvé.");
            }

            // Vérifie la validité des données
            TryValidateModel(patientItem);
            if (!ModelState.IsValid)
            {
                // Crée la liste des erreurs provenant de la validation des données.
                var errorList = (from item in ModelState where item.Value.Errors.Any() select item.Value.Errors[0].ErrorMessage).ToList();

                _logger.LogWarning("Create : Données du patient invalides.");

                // Si la validation échoue, retourne 400 + liste des erreurs (JSON)
                return BadRequest(errorList);
            }

            // Mise à jour du patient et du paramètre associé 
            _context.PatientItems.Update(patientItem);
            _context.ParametreItems.Update(new ParametreItem(patientItem));
            _context.SaveChanges();

            _logger.LogInformation("Update : mise à jour patient {id}.", patientItem.Id);

            return NoContent();
        }
        
        // Le D de CRUD
        // DELETE: api/patient/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            PatientItem patientItem = _context.PatientItems
                            .Include("Admin")
                            .Include("Biometrie")
                            .Include("Const_biologique")
                            .Include("Parametres")
                            .Include("Assuetudes")
                            .FirstOrDefault(t => t.Id == id);

            // Idempotence : le Delete doit avoir le même résultat que le patient soit trouvé ou pas.
            // int patientExists = _context.PatientItems.Count(t => t.Id == id);
            if (patientItem == null) {
                _logger.LogInformation("Delete : suppression du patient {id}.", id);
                return NoContent();
            }

            // Suppression du patient et du paramètre associé 
            _context.PatientItems.Remove(patientItem);
            _context.ParametreItems.Remove(new ParametreItem(patientItem));
            _context.SaveChanges();

            _logger.LogInformation("Delete : suppression du patient {id}.", id);

            return NoContent();
        }

        // La liste des paramètres disponibles (nom des propriétés) et les détails sur celle-ci (unités, min, max ..)
        // GET: api/patient/info
        [HttpGet("info")]
        public IActionResult Info()
        {
            _logger.LogInformation("Info : paramètres disponibles (patient).");

            return new ObjectResult(_context.ParametreInfos.Where( p => p.Groupe == GroupeInfo.Patient || p.Groupe == GroupeInfo.PatientEtParamètre).ToList());
        }

    }
}
