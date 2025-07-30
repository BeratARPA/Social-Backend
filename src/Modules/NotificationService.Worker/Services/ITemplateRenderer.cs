namespace NotificationService.Worker.Services
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string templateName, object model);
    }
}
