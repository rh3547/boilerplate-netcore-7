using Nukleus.Application.Common.Services;
using Nukleus.Domain.Entities;

namespace Nukleus.Infrastructure.Common.Authentication
{
    public class Session : ISession
    {
        private User? User;
        private Account? Account;
        private Dictionary<string, string> SessionValues = new Dictionary<string, string>();

        public User? GetUser()
        {
            return User;
        }

        public void SetUser(User user)
        {
            User = user;
        }

        public Account? GetAccount()
        {
            return Account;
        }

        public void SetAccount(Account? account)
        {
            Account = account;
        }

        public string? GetSessionValue(string key)
        {
            if (SessionValues.ContainsKey(key))
            {
                return SessionValues[key];
            }

            return null;
        }

        public void SetSessionValue(string key, string value)
        {
            SessionValues.Add(key, value);
        }

        public void RemoveSessionValue(string key)
        {
            SessionValues.Remove(key);
        }

        public void ClearAllSessionValues()
        {
            SessionValues.Clear();
        }
    }
}