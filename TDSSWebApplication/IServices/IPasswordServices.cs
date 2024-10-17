using TDSSWebApplication.Models;

namespace TDSSWebApplication.IServices
{
    public interface IPasswordServices
    {
        bool VerifyPassword(Employee user, string password);
    }
}
