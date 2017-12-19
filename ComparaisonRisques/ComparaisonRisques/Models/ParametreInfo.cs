using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ComparaisonRisques.Models
{

    public enum GroupeInfo { Patient = 1, Paramètre = 2, PatientEtParamètre = 3 };

    public class ParametreInfo
    {

        [JsonIgnore]
        public int Id { get; set; }

        public string Titre { get; set; }
        public string Nom { get; set; }
        public string Parent { get; set; }
        public string Unite { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }

        [JsonIgnore]
        public GroupeInfo Groupe { get; set; }

        public ParametreInfo() { }

        /// <summary>
        /// Crée un objet permettant de décrire les paramètres disponibles.
        /// </summary>
        /// <param name="titre">Titre du paramètre</param>
        /// <param name="nom">Nom du paramètre</param>
        /// <param name="parent">Nom de l'objet parent</param>
        /// <param name="unite">Unité du paramètre</param>
        /// <param name="min">Valeure minimale autorisée</param>
        /// <param name="max">Valeure maximale autorisée</param>
        public ParametreInfo(string titre, string nom, string parent, string unite, string min, string max, GroupeInfo groupe)
        {
            Titre = titre;
            Nom = nom;
            Parent = parent;
            Unite = unite;
            Min = min;
            Max = max;
            Groupe = groupe;
        }

    }
}
