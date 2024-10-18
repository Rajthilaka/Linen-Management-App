using TDSSWebApplication.Models;

namespace TDSSWebApplication.IServices
{
    public interface ITokenServices
    {
        string CreateJwtToken(Employee employee);
        string CreateRefreshToken();
    }
}
