using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace ComparaisonRisques.Models
{
    // Liaison de la classe PatientItem avec le contexte DB [EF] 
    // une table de patients, PatientItems
    // une table de paramètres, ParametreItem
    public class MyContext : DbContext
    {
      
        public MyContext(DbContextOptions<MyContext> options) : base(options) {}
        public DbSet<PatientItem> PatientItems { get; set; }
        public DbSet<ParametreItem> ParametreItems { get; set; }

        public override int SaveChanges()
        {
            // La fonction SaveChanges est réécrite afin de lier directement les modifications faites 
            // sur les patients ave les paramètres correspondants.
            
            // Ajoute les paramètres à partir de la liste des patients qui ont été ajoutés.
            this.ParametreItems.AddRange(
                this.ChangeTracker.Entries<PatientItem>()
                .Where(p=>p.State==EntityState.Added)
                .Select(p=> new ParametreItem((PatientItem)p.CurrentValues.ToObject())).ToList());

            // Modifie les paramètres à partir de la liste des patients qui ont été modifiés.
            this.ParametreItems.UpdateRange(
                 this.ChangeTracker.Entries<PatientItem>()
                 .Where(p => p.State == EntityState.Modified)
                 .Select(p => new ParametreItem((PatientItem)p.CurrentValues.ToObject())).ToList());

            // Supprime les paramètres à partir de la liste des patients qui ont été supprimés.
            this.ParametreItems.RemoveRange(
                this.ChangeTracker.Entries<PatientItem>()
                .Where(p => p.State == EntityState.Deleted)
                .Select(p => new ParametreItem((PatientItem)p.CurrentValues.ToObject())).ToList());

            return base.SaveChanges();

        }

    }
}
