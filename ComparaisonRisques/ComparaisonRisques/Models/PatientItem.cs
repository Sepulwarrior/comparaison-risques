using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComparaisonRisques.Models
{
    //--- 
    public struct Admin
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }
        public DateTime Date_de_naissance { get; set; }
        public string Genre { get; set; }
    }

    public struct Biometrie
    {
        public int Poids { get; set; }
        public int Taille { get; set; }
    }

    public struct ConstBiologique
    {
        public double HbA1c { get; set; }
        public int Cholesterol_total { get; set; }
        public int Cholesterol_HDL { get; set; }
    }

    public struct Parametres
    {
        public int PSS { get; set; }
    }

    public struct Assuetudes
    {
        public int Consommation_tabagique { get; set; }
    }

    public class PatientItem
    {
        public int Id { get; set; }
        public Admin Admin { get; set; }
        public Biometrie Biometrie { get; set; }
        public ConstBiologique Const_biologique { get; set; }
        public Parametres Parametres { get; set; }
        public Assuetudes Assuetudes { get; set; }
    }

}
