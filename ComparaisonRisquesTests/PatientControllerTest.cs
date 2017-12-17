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

namespace ComparaisonRisquesTests
{
    class PatientControllerTest
    {

        private readonly PatientController _crontrollerTest;
        private readonly MyContext _contextTest;

        public PatientControllerTest()
        {
            var loggerTest = new Mock<ILogger<PatientController>>().Object;
            _contextTest = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            _crontrollerTest = new PatientController(loggerTest, _contextTest);
        }

        [Test]
        public void PatientControllerCreateOK()
        {
            PatientItem patientItem = _contextTest.PatientItems.FirstOrDefault();
            patientItem.Id = 0;
            //var retour = _crontrollerTest.Create(patientItem);
            //Assert.That(typeof(List<ParametreItem>), Is.EqualTo(retour.GetType()));
            Assert.AreEqual(5, _contextTest.PatientItems.Count());
        }

        [Test]
        public void PatientControllerCreateBadRequestNull()
        {
            var retour = _crontrollerTest.Create(null);
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerCreateBadRequestWrongWeight()
        {
            PatientItem patientItem = _contextTest.PatientItems.FirstOrDefault();
            patientItem.Id = 0;
            var bio = patientItem.Biometrie;
            bio.Poids = 163;
            patientItem.Biometrie = bio;
            var retour = _crontrollerTest.Create(patientItem);
     
            Assert.That(typeof(BadRequestResult), Is.EqualTo(retour.GetType()));
            
        }


        [Test]
        public void PatientControllerReadOk()
        {
            var retour = _crontrollerTest.Read("a",2);
            Assert.That(typeof(List<PatientItem>), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void PatientControllerReadIdOk()
        {
            var retour = _crontrollerTest.Read(163);
            Assert.That(typeof(ObjectResult), Is.EqualTo(retour.GetType()));
        }





        [Test]
        public void PatientControllerDeleteOk()
        {
            var retour = _crontrollerTest.Delete(163);
            Assert.That(typeof(NoContentResult), Is.EqualTo(retour.GetType()));
        }

    }
}
