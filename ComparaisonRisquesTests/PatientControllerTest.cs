using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using ComparaisonRisques.Controllers;
using ComparaisonRisques.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Newtonsoft.Json;

namespace ComparaisonRisquesTests
{
    class PatientControllerTest
    {

        private readonly PatientController _crontrollerTest;
        private readonly MyContext _contextTest;
        private readonly ILogger<PatientController> _loggerTest;

        public PatientControllerTest()
        {
            var loggerTest = new Mock<ILogger<PatientController>>().Object;

            _loggerTest = loggerTest;
            _contextTest = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            _contextTest.EnsureSeeded();
            _crontrollerTest = new PatientController(loggerTest, _contextTest);
        }

        [Test]
        public void PatientControllerCreateOK()
        {
            // Utilisation d'un DBContext isolé pour les opérations modifiant la base de donnée
            var testContext = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            testContext.EnsureSeeded();
            var testPatientController = new PatientController(_loggerTest, testContext);

            string jsonData = "{'id':0," +
                "'admin':{'prenom':'Frédérik','nom':'Liénard','date_de_naissance':'1983-07-21T06:45:13Z','Genre':'Male'}," +
                "'biometrie':{'poids':78,'taille':175}," +
                "'const_biologique':{'HbA1c':0.1,'Cholesterol_total':171,'Cholesterol_HDL':68}," +
                "'parametres':{'PSS':106}," +
                "'assuetudes':{'Consommation_tabagique':0}}";
            PatientItem patientItem = JsonConvert.DeserializeObject<PatientItem>(jsonData);

            var objectValidator = new Mock<Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            testPatientController.ObjectValidator = objectValidator.Object;

            var retour = testPatientController.Create(patientItem);
            
            Assert.That(typeof(CreatedAtRouteResult), Is.EqualTo(retour.GetType()));

        }

        [Test]
        public void PatientControllerCreateBadRequestNull()
        {
            var retour = _crontrollerTest.Create(null);
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

        // Ce test ne fonctionne pas comme attendu
        // je n'ai pas encore trouvé le moyen de tester la validation de données
        [Test]
        public void PatientControllerCreateBadRequestWrongWeight()
        {

            // Utilisation d'un DBContext isolé pour les opérations modifiant la base de donnée
            var testContext = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            testContext.EnsureSeeded();
            var testPatientController = new PatientController(_loggerTest, testContext);

            string jsonData = "{'id':0,"+
                "'admin':{'prenom':'Frédérik','nom':'Liénard','date_de_naissance':'1983-07-21T06:45:13Z','Genre':'Male'},"+
                "'biometrie':{'poids':178,'taille':175},"+
                "'const_biologique':{'HbA1c':0.1,'Cholesterol_total':171,'Cholesterol_HDL':68},"+
                "'parametres':{'PSS':106},"+
                "'assuetudes':{'Consommation_tabagique':0}}";
            PatientItem patientItem = JsonConvert.DeserializeObject<PatientItem>(jsonData);
            
            var retour = testPatientController.Create(patientItem);
     
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
            
        }

        [Test]
        public void PatientControllerReadOk()
        {
            var retour = _crontrollerTest.Read("a","2");
            Assert.That(typeof(ObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerReadIdOk()
        {

            var retour = _crontrollerTest.Read(163);
            
            Assert.That(typeof(ObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerUpdateOk()
        {

            // Utilisation d'un DBContext isolé pour les opérations modifiant la base de donnée
            var testContext = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            testContext.EnsureSeeded();
            var testPatientController = new PatientController(_loggerTest, testContext);

            string jsonData = "{'id':5," +
                "'admin':{'prenom':'Frédérik','nom':'Liénard','date_de_naissance':'1983-07-21T06:45:13Z','Genre':'Male'}," +
                "'biometrie':{'poids':78,'taille':175}," +
                "'const_biologique':{'HbA1c':0.1,'Cholesterol_total':171,'Cholesterol_HDL':68}," +
                "'parametres':{'PSS':106}," +
                "'assuetudes':{'Consommation_tabagique':0}}";
            PatientItem patientItem = JsonConvert.DeserializeObject<PatientItem>(jsonData);

            var objectValidator = new Mock<Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            testPatientController.ObjectValidator = objectValidator.Object;

            var retour = testPatientController.Update(5, patientItem);

            Assert.That(typeof(NoContentResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerUpdateBadRequest()
        {

            // Utilisation d'un DBContext isolé pour les opérations modifiant la base de donnée
            var testContext = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            var testPatientController = new PatientController(_loggerTest, testContext);

            string jsonData = "{'id':5," +
                "'admin':{'prenom':'Frédérik','nom':'Liénard','date_de_naissance':'1983-07-21T06:45:13Z','Genre':'Male'}," +
                "'biometrie':{'poids':78,'taille':175}," +
                "'const_biologique':{'HbA1c':0.1,'Cholesterol_total':171,'Cholesterol_HDL':68}," +
                "'parametres':{'PSS':106}," +
                "'assuetudes':{'Consommation_tabagique':0}}";
            PatientItem patientItem = JsonConvert.DeserializeObject<PatientItem>(jsonData);

            var objectValidator = new Mock<Microsoft.AspNetCore.Mvc.ModelBinding.Validation.IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            testPatientController.ObjectValidator = objectValidator.Object;

            var retour = testPatientController.Update(1, patientItem);

            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerDeleteExistsOk()
        {

            // Utilisation d'un DBContext isolé pour les opérations modifiant la base de donnée
            var testContext = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            var testPatientController = new PatientController(_loggerTest, testContext);

            var retour = testPatientController.Delete(1);
            Assert.That(typeof(NoContentResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerDeleteNotExistsOk()
        {
            var retour = _crontrollerTest.Delete(502);
            Assert.That(typeof(NoContentResult), Is.EqualTo(retour.GetType()));
        }

    }
}
