using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inflector;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace MeetupSurvey.API
{
    public class CustomPluralizer : IPluralizer
    {
        public string Pluralize(string identifier)
        {
            Inflector.Inflector inflector = new Inflector.Inflector(CultureInfo.CurrentCulture);
            return inflector.Pluralize(identifier) ?? identifier;
        }

        public string Singularize(string identifier)
        {
            Inflector.Inflector inflector = new Inflector.Inflector(CultureInfo.CurrentCulture);
            return inflector.Singularize(identifier) ?? identifier;
        }
    }

    public class MyDesignTimeServices : IDesignTimeServices
    {
        public void ConfigureDesignTimeServices(IServiceCollection services)
        {
            services.AddSingleton<IPluralizer, CustomPluralizer>();
        }
    }
}
