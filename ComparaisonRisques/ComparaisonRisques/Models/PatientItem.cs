using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public class Admin
    {
        [JsonIgnore]
        [Key, ForeignKey("PatientItem")]
        public int Id { get; set; }

        public string Prenom { get; set; }
        public string Nom { get; set; }

        // Mettre la date sous format ISO 8601, le format mis à l'origine (31/12/2017) prête à confusion au niveau jour<>mois
        [Range(typeof(DateTime), "1930-01-01T00:00:00Z", "2017-12-31T00:00:00Z", ErrorMessage = "La date de naissance doit être comprise entre 1/1/1930 et 31/12/2017.")]
        public DateTime Date_de_naissance { get; set; }

        [GenreValide(ErrorMessage = "Le genre doit être Male ou Female.")]
        public string Genre { get; set; }

        // Champ calculé : Age
        [JsonIgnore]
        [NotMapped]
        public int Age { get { return (DateTime.Now - Date_de_naissance).Days / 365; } }

    }

    public class Biometrie
    {
        [JsonIgnore]
        [Key, ForeignKey("PatientItem")]
        public int Id { get; set; }

        [Range(45, 140, ErrorMessage = "Le poids doit être compris entre 45 et 140 kg.")]
        public int Poids { get; set; }

        [Range(155, 195, ErrorMessage = "La taille doit être comprise entre 155 et 195 cm.")]
        public int Taille { get; set; }

        // Champ calculé : BMI (IMC) = poids / taille ²
        [JsonIgnore]
        [NotMapped]
        public int BMI { get { return (int)Math.Round(Poids / Math.Pow(Taille / 100.0, 2)); }}

    }

    public class ConstBiologique
    {
        [JsonIgnore]
        [Key, ForeignKey("PatientItem")]
        public int Id { get; set; }

        [Range(0.05, 0.12, ErrorMessage = "La HbA1c doit être compris entre 0.05 et 0.12 ).")]
        public double HbA1c { get; set; }

        [Range(130, 320, ErrorMessage = "Le cholesterol total doit être compris entre 130 et 320 mg/dl).")]
        public int Cholesterol_total { get; set; }

        [Range(20, 100, ErrorMessage = "Le cholesterol HDL doit être compris entre 20 et 100 mg/dl).")]
        public int Cholesterol_HDL { get; set; }

        // Champ calculé : Cholesterol, Total = HDC + HDL
        [JsonIgnore]
        [NotMapped]
        public int Cholesterol_HDC { get { return Cholesterol_total - Cholesterol_HDL; } }

    }

    public class Parametres
    {
        [JsonIgnore]
        [Key, ForeignKey("PatientItem")]
        public int Id { get; set; }

        [Range(90, 200, ErrorMessage = "La PSS doit être comprise entre 90 et 200 mmHg.")]
        public int PSS { get; set; }
    }

    public class Assuetudes
    {
        [JsonIgnore]
        [Key,ForeignKey("PatientItem")]
        public int Id { get; set; }

        [Range(0, 80, ErrorMessage = "La consommation tabagique doit être comprise entre 0 et 80 paquets par année.")]
        public int Consommation_tabagique { get; set; }
    }

    public class PatientItem
    {
        [Key]
        public int Id { get; set; }
        public Admin Admin { get; set; }
        public Biometrie Biometrie { get; set; }
        public ConstBiologique Const_biologique { get; set; }
        public Parametres Parametres { get; set; }
        public Assuetudes Assuetudes { get; set; }
    }

}
