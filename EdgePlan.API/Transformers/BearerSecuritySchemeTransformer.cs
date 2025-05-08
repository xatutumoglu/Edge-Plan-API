
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;        

namespace EdgePlan.API.Transformers
{
    public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??= 
                new System.Collections.Generic.Dictionary<string, OpenApiSecurityScheme>();
            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "JWT formatında: \"Bearer {token}\""
            };

            document.SecurityRequirements ??= 
                new System.Collections.Generic.List<OpenApiSecurityRequirement>();
            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
            {
                [ new OpenApiSecurityScheme 
                    { Reference = new OpenApiReference 
                        { Type = ReferenceType.SecurityScheme, Id = "Bearer" } 
                    }
                ] = Array.Empty<string>()
            });

            return Task.FromResult(document);
        }
    }
}