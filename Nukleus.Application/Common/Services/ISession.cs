using Nukleus.Domain.Entities;

namespace Nukleus.Application.Common.Services
{
    public interface ISession
    {
        public User? GetUser();
        public void SetUser(User user);
        public Account? GetAccount();
        public void SetAccount(Account? account);
        public string? GetSessionValue(string key);
        public void SetSessionValue(string key, string value);
        public void RemoveSessionValue(string key);
        public void ClearAllSessionValues();
    }
}