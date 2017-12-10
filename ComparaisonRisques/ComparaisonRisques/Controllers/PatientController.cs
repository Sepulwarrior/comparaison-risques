using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ComparaisonRisques.Models;
using Newtonsoft.Json;

namespace ComparaisonRisques.Controllers
{
    [Produces("application/json")]
    [Route("api/Patient")]
    public class PatientController : Controller
    {

        private readonly PatientContext _context;

        public PatientController(PatientContext context)
        {
            _context = context;

            if (_context.PatientItems.Count() == 0)
            {
                // Insertion des données de tests
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
            if (patientItem == null) { return BadRequest(); }

            // l'auto-incrément fonctionne lorsque la base de donnée est vide.
            // avec l'insertion des données de tests, il tente de créer la première entité avec l'Id 1 ( les 500 premiers existent)
            // j'ai donc contourné l'auto-incrément en faisant max+1 ci dessous
            if (patientItem.Id == 0) { patientItem.Id = _context.PatientItems.Max(t => t.Id) + 1; }

            // Définir ce qu'on doit faire si l'entité existe déjà
            var patientExist = _context.PatientItems.FirstOrDefault(t => t.Id == patientItem.Id);

            _context.PatientItems.Add(patientItem);
            _context.SaveChanges();

            return CreatedAtRoute("Read", new { id = patientItem.Id }, patientItem);
        }

        // Le R du CRUD : Sans ID, renvoie toute les entités
        // GET: api/patient
        [HttpGet]
        public IEnumerable<PatientItem> Read()
        {
            return _context.PatientItems.ToList();
        }

        // Le R du CRUD : Avec ID, renvoie l'entité demandée
        // GET: api/patient/5
        [HttpGet("{id}", Name = "Read")]
        public IActionResult Read(int id)
        {
            PatientItem patientItem = _context.PatientItems.FirstOrDefault(t => t.Id == id);

            if (patientItem == null) { return NotFound();}

            return new ObjectResult(patientItem);
        }
        
        // Le U de CRUD
        // PUT: api/patient/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]PatientItem patientItem)
        {
            if (patientItem == null || patientItem.Id != id) { return BadRequest(); }

            PatientItem patientExists = _context.PatientItems.FirstOrDefault(t => t.Id == id);
            if (patientExists == null) { return NotFound(); }

            _context.PatientItems.Update(patientItem);
            _context.SaveChanges();

            return new NoContentResult();
        }
        
        // Le D de CRUD
        // DELETE: api/patient/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            PatientItem patientItem = _context.PatientItems.FirstOrDefault(t => t.Id == id);
            if (patientItem == null) { return NotFound(); }

            _context.PatientItems.Remove(patientItem);
            _context.SaveChanges();

            return new NoContentResult();
        }
    }
}
