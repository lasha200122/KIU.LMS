namespace KIU.LMS.Domain.Common.Utils;

public static class TextGenerator
{
    public static string GenerateRandomPassword(int length)
    {
        var password = new StringBuilder();
        var random = RandomNumberGenerator.Create();

        password.Append(GetRandomChar(CharacterConstants.LowercaseChars, random));
        password.Append(GetRandomChar(CharacterConstants.UppercaseChars, random));
        password.Append(GetRandomChar(CharacterConstants.NumberChars, random));
        password.Append(GetRandomChar(CharacterConstants.SpecialChars, random));

        var allChars = CharacterConstants.LowercaseChars + CharacterConstants.UppercaseChars + CharacterConstants.NumberChars + CharacterConstants.SpecialChars;
        for (int i = password.Length; i < length; i++)
        {
            password.Append(GetRandomChar(allChars, random));
        }

        return new string(password.ToString().ToCharArray().OrderBy(x => GetNextInt32(random)).ToArray());
    }

    public static string GenerateRandomToken(int length)
    {
        var password = new StringBuilder();
        var random = RandomNumberGenerator.Create();

        password.Append(GetRandomChar(CharacterConstants.LowercaseChars, random));
        password.Append(GetRandomChar(CharacterConstants.UppercaseChars, random));
        password.Append(GetRandomChar(CharacterConstants.NumberChars, random));

        var allChars = CharacterConstants.LowercaseChars + CharacterConstants.UppercaseChars + CharacterConstants.NumberChars;
        for (int i = password.Length; i < length; i++)
        {
            password.Append(GetRandomChar(allChars, random));
        }

        return new string(password.ToString().ToCharArray().OrderBy(x => GetNextInt32(random)).ToArray());
    }

    public static char GetRandomChar(string chars, RandomNumberGenerator random)
    {
        return chars[GetNextInt32(random) % chars.Length];
    }

    public static int GetNextInt32(RandomNumberGenerator random)
    {
        byte[] bytes = new byte[4];
        random.GetBytes(bytes);
        return BitConverter.ToInt32(bytes, 0) & int.MaxValue;
    }
}
