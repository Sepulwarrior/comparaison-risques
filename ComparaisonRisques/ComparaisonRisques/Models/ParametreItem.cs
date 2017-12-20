using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ComparaisonRisques.Models
{
    public class ParametreItem
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int Age { get; set; }
        public int BMI { get; set; }
        public double HbA1c { get; set; }
        public int Cholesterol_total { get; set; }
        public int Cholesterol_HDC { get; set; }
        public int PSS { get; set; }
        public int Consommation_tabagique { get; set; }

        public ParametreItem(){ }

        /// <summary>
        /// Crée un objet ParametreItem sur base des données copiée ou calculée d'un objet patientItem
        /// </summary>
        /// <param name="patientItem">Fiche patient</param>
        public ParametreItem(PatientItem patientItem) {
            Id = patientItem.Id;
            Age = patientItem.Admin.Age;
            BMI = patientItem.Biometrie.BMI;
            HbA1c = patientItem.Const_biologique.HbA1c;
            Cholesterol_total = patientItem.Const_biologique.Cholesterol_total;
            Cholesterol_HDC = patientItem.Const_biologique.Cholesterol_HDC;
            PSS = patientItem.Parametres.PSS;
            Consommation_tabagique = patientItem.Assuetudes.Consommation_tabagique;
        }

    }
}
