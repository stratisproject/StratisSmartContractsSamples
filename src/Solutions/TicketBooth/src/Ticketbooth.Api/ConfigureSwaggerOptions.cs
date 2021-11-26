using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using System.Reflection;

namespace Ticketbooth.Api
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // enables request/response examples
            options.OperationFilter<ExamplesOperationFilter>();

            // Swagger documentation
            var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            options.IncludeXmlComments(Path.Combine(basePath, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            options.AddFluentValidationRules();

            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info()
            {
                Title = "Ticketbooth Full Node API",
                Version = description.ApiVersion.ToString(),
                License = new License
                {
                    Name = "MIT",
                    Url = "https://github.com/ticketbooth-solutions/Ticketbooth.Documentation/blob/master/LICENSE"
                },
                Contact = new Contact
                {
                    Name = "Adam Shirt",
                    Email = "adam.shirt@developmomentum.com",
                    Url = "https://github.com/ticketbooth-solutions/Ticketbooth.Documentation"
                },
                Description = "An extension to the Stratis full node web API, for interacting with the Ticketbooth smart contract."
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
