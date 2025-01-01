namespace KIU.LMS.Domain.Common.Constants.Hash;

public static class HashConstants
{
    public const int keySize = 64;
    public const int iterations = 350000;
    public static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
}