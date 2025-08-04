using RazorLight;
using System.Reflection;

namespace NotificationService.Worker.Services
{
    public class TemplateRenderer : ITemplateRenderer
    {
        private readonly RazorLightEngine _engine;

        public TemplateRenderer()
        {
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(Assembly.GetExecutingAssembly(), "NotificationService.Worker.Templates")
                .UseMemoryCachingProvider()
                .SetOperatingAssembly(Assembly.GetExecutingAssembly())
                .EnableDebugMode()
                .Build();

        }

        public async Task<string> RenderAsync(string templateName, object model)
        {
            var templateKey = $"{templateName}.cshtml";
            return await _engine.CompileRenderAsync(templateKey, model);
        }
    }
}
