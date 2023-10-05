namespace Nukleus.Application.Common.Services
{
    public interface IHashingService
    {
        string Hash(string password);

        bool Verify(string password, string hashedPassword);
    }
}