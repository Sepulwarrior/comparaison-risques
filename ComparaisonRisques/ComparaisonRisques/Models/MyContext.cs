using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComparaisonRisques.Models
{
    // Liaison de la classe PatientItem avec le contexte DB [EF] 
    // une table de patients, PatientItems
    // une table de paramètres, ParametreItem
    // une table d'information pour les paramètres, ParametreItem
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) {}
        public DbSet<PatientItem> PatientItems { get; set; }
        public DbSet<ParametreItem> ParametreItems { get; set; }
        public DbSet<ParametreInfo> ParametreInfos { get; set; }

        /// <summary>
        /// Insertion des données lors de la création d'une nouvelle DB
        /// </summary>
        public void EnsureSeeded()
        {

            // Insertion des données de tests ( fiches patient )
            if (PatientItems.Count() == 0)
            {
                if (System.IO.File.Exists(@"data/mock_data.json"))
                {
                    string jsonData = System.IO.File.ReadAllText(@"data/mock_data.json");

                    List<PatientItem> patientItems = JsonConvert.DeserializeObject<List<PatientItem>>(jsonData);
                    List<ParametreItem> parametreItems = patientItems.Select(p => new ParametreItem(p)).ToList();

                    PatientItems.AddRange(patientItems);
                    ParametreItems.AddRange(parametreItems);

                    SaveChanges();
                }
            }

            // Ajout des paramètres à la première utilisation
            if (ParametreInfos.Count() == 0)
            {
                ParametreInfos.AddRange(
                        new ParametreInfo("Prénom", "prenom", "admin", "","","", GroupeInfo.Patient),
                        new ParametreInfo("Nom", "nom", "admin", "","","", GroupeInfo.Patient),
                        new ParametreInfo("Date de naissance", "date_de_naissance", "admin", "", "1930-01-01T00:00:00Z", "2017-12-31T00:00:00Z", GroupeInfo.Patient),
                        new ParametreInfo("Genre", "genre", "admin", "","","", GroupeInfo.Patient),
                        new ParametreInfo("Poids", "poids", "biometrie", "kg", "45","140", GroupeInfo.Patient),
                        new ParametreInfo("Taille", "taille", "biometrie", "cm","155","195", GroupeInfo.Patient),
                        new ParametreInfo("Hémoglobine glyquée", "hbA1c", "const_biologique", "ratio", "0.05", "0.12", GroupeInfo.PatientEtParamètre),
                        new ParametreInfo("Cholesterol total", "cholesterol_total", "const_biologique", "mg/dl", "130","320", GroupeInfo.PatientEtParamètre),
                        new ParametreInfo("Cholesterol HDL", "cholesterol_HDL", "const_biologique", "mg/dl", "20","100", GroupeInfo.Patient),
                        new ParametreInfo("Pression sanguine systolique", "pss", "admin", "mmHg", "90","200", GroupeInfo.PatientEtParamètre),
                        new ParametreInfo("Consommation tabagique", "consommation_tabagique", "assuetudes", "nombre de paquets par année", "0","80", GroupeInfo.PatientEtParamètre),
                        new ParametreInfo("Age", "age", "", "années", "0","88", GroupeInfo.Paramètre),
                        new ParametreInfo("Indice de masse corporelle", "bmi", "", "kg/m²", "11","59", GroupeInfo.Paramètre),
                        new ParametreInfo("Cholesterol HDC", "cholesterol_HDC", "", "mg/dl", "30","300", GroupeInfo.Paramètre)
                    );

                SaveChanges();
            }
        }

    }
}
