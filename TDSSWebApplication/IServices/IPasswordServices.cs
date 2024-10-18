using TDSSWebApplication.Models;

namespace TDSSWebApplication.IServices
{
    public interface IPasswordServices
    {
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}
