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

namespace ComparaisonRisquesTests
{

    [TestFixture]
    class ParametreControllerTest
    {

        private readonly ParametreController _crontrollerTest;

        public ParametreControllerTest() {
            var loggerTest = new Mock<ILogger<ParametreController>>().Object;
            var contextTest = new MyContext(new DbContextOptionsBuilder<MyContext>().UseInMemoryDatabase("CompaRisquesTest").Options);
            _crontrollerTest = new ParametreController(loggerTest, contextTest);
        }

        [Test]
        public void ParametreControllerGetStatsOK()
        {
            var retour = _crontrollerTest.GetStats();
            Assert.That(typeof(List<ParametreItem>), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerInfoOK()
        {
            var retour = _crontrollerTest.Info();
            Assert.That(typeof(ObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerLineChartOK()
        {
            var retour = _crontrollerTest.LineChart("Age", "BMI");
            Assert.That(typeof(ObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerLineChartBadRequestAbscisse()
        {
            var retour = _crontrollerTest.LineChart("Abscisse", "BMI");
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerLineChartBadRequestOrdonnee()
        {
            var retour = _crontrollerTest.LineChart("Age", "Ordonnee");
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerScatterChartOK()
        {
            var retour = _crontrollerTest.ScatterChart("Age", "BMI");
            Assert.That(typeof(ObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerScatterChartBadRequestAbscisse()
        {
            var retour = _crontrollerTest.ScatterChart("Abscisse", "BMI");
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

        [Test]
        public void ParametreControllerScatterChartBadRequestOrdonnee()
        {
            var retour = _crontrollerTest.ScatterChart("Age", "Ordonnee");
            Assert.That(typeof(BadRequestObjectResult), Is.EqualTo(retour.GetType()));
        }

    }
}
