using System.Threading.Tasks;

namespace Server.Services
{
  public interface ISmsSender
  {
    Task SendSmsAsync(string number, string message);
  }
}
