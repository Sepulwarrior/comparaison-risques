using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.EntityFrameworkCore;
using ComparaisonRisques.Models;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;


namespace ComparaisonRisques
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Démarrage de la base de donnée SQLite
            using (var client = new MyContext(new DbContextOptionsBuilder<MyContext>().UseSqlite("Filename=data/comparaison-risques.db").Options))
            {
                client.Database.EnsureCreated();
                client.EnsureSeeded();
            }

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();
            services.AddDbContext<MyContext>(opt => opt.UseSqlite("Filename=data/comparaison-risques.db"));

            // Définition du document Swagger
            services.AddSwaggerGen(c => c.SwaggerDoc("prototype", new Info { Title = "API de comparaison des risques", Version = "prototype" }));

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Définition de l'emplacement des fichers logs
            loggerFactory.AddFile("log/comparaison-risques_{Date}.txt");

            // Pour éviter le blocage des requêtes multiorigines (client test en local)
            app.UseCors(builder => builder  .AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod());

            // Activation de Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/prototype/swagger.json", "API de comparaison des risques."));

            app.UseMvc();

        }
    }
}
