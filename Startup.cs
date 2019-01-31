﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using communication.Interfaces;
using communication.MessageRouter;
using communication.Models;
using communication.Mongo;
using communication.Rabbit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace communication
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddSingleton<IConfiguration>(Configuration);
      services.AddSingleton<RabbitConnection>();
      services.AddSingleton<RabbitManager>();
      services.AddSingleton<Router>();
      // services.AddSingleton<IDatabase<Debtor>, MongoDatabase<Debtor>>((ctx) =>
      // {
      //     MongoConnection connection = ctx.GetRequiredService<MongoConnection>();
      //     return new MongoDatabase<Debtor>(connection, "Debtor");
      // });
      // services.AddSingleton<MongoConnection>();
    }

    // This method gets called by the runtime. Use this met23hod to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }
      app.ApplicationServices.GetService<RabbitManager>();
      app.ApplicationServices.GetServices<Router>();
      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
