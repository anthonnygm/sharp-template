using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(string subject, string message, string email);
}
