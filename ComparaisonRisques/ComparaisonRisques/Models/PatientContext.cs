using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace ComparaisonRisques.Models
{
    // Liaison de la classe PatientItem avec le contexte DB [EF] ( avec table correspondante, PatientItems )
    public class PatientContext : DbContext
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options) {}
        public DbSet<PatientItem> PatientItems { get; set; }
    }
}
