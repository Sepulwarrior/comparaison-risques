using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ComparaisonRisques.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace ComparaisonRisques.Controllers
{
    [Produces("application/json")]
    [Route("api/patient")]
    public class PatientController : Controller
    {

        private readonly PatientContext _context;
        private readonly ILogger _logger;

        public PatientController(ILogger<PatientController> logger,PatientContext context)
        {
            _logger = logger;
            _context = context;

            if (_context.PatientItems.Count() == 0)
            {
                // TODO : Trancher si c'est le bon endroit pour faire ça !!
                // Insertion des données de tests
                _logger.LogInformation("Insertion des données de tests depuis MOCK_DATA.JSON");
                string jsonData = System.IO.File.ReadAllText(@"MOCK_DATA.JSON");
                _context.AddRange(JsonConvert.DeserializeObject<List<PatientItem>>(jsonData));
                _context.SaveChanges();
            }
        }

        // Le C du CRUD : renvoie l'entité crée
        // POST: api/patient
        [HttpPost]
        public IActionResult Create([FromBody]PatientItem patientItem)
        {
            if (patientItem == null) {
                _logger.LogWarning("Update : Patient vide ou incomplet.");
                return BadRequest("Patient vide ou incomplet.");
            }

            // l'auto-incrément fonctionne lorsque la base de donnée est vide.
            // avec l'insertion des données de tests, il tente de créer la première entité avec l'Id 1 ( les 500 premiers existent )
            // j'ai donc contourné l'auto-incrément en faisant max+1 ci dessous
            if (patientItem.Id == 0) { patientItem.Id = _context.PatientItems.Max(t => t.Id) + 1; }

            // TODO Définir ce qu'on doit faire si l'entité existe déjà
            var patientExist = _context.PatientItems.FirstOrDefault(t => t.Id == patientItem.Id);

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

            _context.PatientItems.Add(patientItem);
            _context.SaveChanges();

            _logger.LogInformation("Create : Patient créé : {id} ", patientItem.Id);

            return CreatedAtRoute("Read", new { id = patientItem.Id }, patientItem);
        }

        // Le R du CRUD : Renvoie toute les entités
        // GET: api/patient
        [HttpGet]
        public IEnumerable<PatientItem> Read(string search, int limit)
        {

            var patientItems = _context.PatientItems.ToList();

            if (search != null)
            {
                patientItems = patientItems.Where(t => t.Admin.Nom.ToLower().StartsWith(search.ToLower())).ToList();
            }

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
            
            PatientItem patientItem = _context.PatientItems.FirstOrDefault(t => t.Id == id);

            // TODO Vérifier ce qu'on doit faire lorsque non trouvé : objet vide ou 404 ?
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

            _context.PatientItems.Update(patientItem);
            _context.SaveChanges();

            _logger.LogInformation("Update : mise à jour patient {id}.", patientItem.Id);

            return NoContent();
        }
        
        // Le D de CRUD
        // DELETE: api/patient/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            PatientItem patientItem = _context.PatientItems.FirstOrDefault(t => t.Id == id);

            // Idempotence : le Delete doit avoir le même résultat que le patient soit trouvé ou pas.
            int patientExists = _context.PatientItems.Count(t => t.Id == id);
            if (patientExists == 0) {
                _logger.LogInformation("Delete : suppression du patient {id}.", patientItem.Id);
                return NoContent();
            }

            _context.PatientItems.Remove(patientItem);
            _context.SaveChanges();

            _logger.LogInformation("Delete : suppression du patient {id}.", patientItem.Id);

            return NoContent();
        }
    }
}
