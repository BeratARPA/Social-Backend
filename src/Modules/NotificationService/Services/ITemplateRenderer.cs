namespace NotificationService.Services
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string templateName, object model);
    }
}
