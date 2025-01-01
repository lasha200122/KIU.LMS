namespace KIU.LMS.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    public string HashPasword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(HashConstants.keySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            HashConstants.iterations,
            HashConstants.hashAlgorithm,
            HashConstants.keySize);

        return Convert.ToHexString(hash);
    }

    public bool VerifyPassword(string password, string hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, HashConstants.iterations, HashConstants.hashAlgorithm, HashConstants.keySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}
