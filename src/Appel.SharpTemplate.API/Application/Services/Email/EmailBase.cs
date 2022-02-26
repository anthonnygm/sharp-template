using System.IO;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Services;

public abstract class EmailBase
{
    protected virtual async Task<string> LoadEmailTemplateAsync(string templateName)
    {
        using (var sr = File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", $"{templateName}.html")))
        {
            return await sr.ReadToEndAsync();
        }
    }
}
