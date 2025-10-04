using Scriban;

namespace NotificationService.Services
{
    public class TemplateRenderer : ITemplateRenderer
    {
        private readonly string _templateDirectory;

        public TemplateRenderer()
        {
            // Templates klasörünü publish sonrası da doğru bulmak için
            _templateDirectory = Path.Combine(AppContext.BaseDirectory, "Templates");
        }

        public async Task<string> RenderAsync(string templateName, object model)
        {
            var templatePath = Path.Combine(_templateDirectory, $"{templateName}.html");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template dosyası bulunamadı: {templatePath}");

            var templateContent = await File.ReadAllTextAsync(templatePath);

            var template = Template.Parse(templateContent);
            if (template.HasErrors)
            {
                var errorMessages = string.Join(Environment.NewLine, template.Messages.Select(m => m.Message));
                throw new InvalidOperationException($"Şablon ayrıştırma hatası:\n{errorMessages}");
            }

            var result = template.Render(model, member => member.Name);
            return result;
        }
    }
}
