using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace ComparaisonRisques.Models
{
    // Fonction de validation du genre.
    public class GenreValide : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return new String[] { "Male", "Female" }.Contains(value);
        }
    }

    public struct Admin
    {
        public string Prenom { get; set; }
        public string Nom { get; set; }

        [Range(typeof(DateTime), "1/1/1930", "31/12/2017", ErrorMessage = "La date de naissance doit être comprise entre 1/1/1930 et 31/12/2017.")]
        public DateTime Date_de_naissance { get; set; }

        [GenreValide(ErrorMessage = "Le genre doit être Male ou Female.")]
        public string Genre { get; set; }
    }

    public struct Biometrie
    {
        [Range(45,140, ErrorMessage = "Le poids doit être compris entre 45 et 140 kg.")]
        public int Poids { get; set; }

        [Range(155, 195, ErrorMessage = "La taille doit être comprise entre 155 et 195 cm.")]
        public int Taille { get; set; }
    }

    public struct ConstBiologique
    {
        [Range(0.05, 0.12, ErrorMessage = "La HbA1c doit être compris entre 0.05 et 0.12 ).")]
        public double HbA1c { get; set; }

        [Range(130, 320, ErrorMessage = "Le cholesterol total doit être compris entre 130 et 320 mg/dl).")]
        public int Cholesterol_total { get; set; }

        [Range(20, 100, ErrorMessage = "Le cholesterol HDL doit être compris entre 20 et 100 mg/dl).")]
        public int Cholesterol_HDL { get; set; }
    }

    public struct Parametres
    {
        [Range(90, 200, ErrorMessage = "La PSS doit être comprise entre 90 et 200 mmHg.")]
        public int PSS { get; set; }
    }

    public struct Assuetudes
    {
        [Range(0, 80, ErrorMessage = "La consommation tabagique doit être comprise entre 0 et 80 paquets par année.")]
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
