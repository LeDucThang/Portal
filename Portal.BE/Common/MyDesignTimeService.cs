using Bricelam.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Common
{
    public class MyDesignTimeService : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddHandlebarsScaffolding();
            serviceCollection.AddHandlebarsTransformers(
              entityNameTransformer: x => x + "DAO",
              entityFileNameTransformer: x => x + "DAO",
              constructorTransformer: x =>
              {
                  x.PropertyType += "DAO";
                  return x;
              },
              navPropertyTransformer: x =>
              {
                  x.PropertyType += "DAO";
                  return x;
              });
            serviceCollection.AddSingleton<IPluralizer, Pluralizer>();
        }
    }
}