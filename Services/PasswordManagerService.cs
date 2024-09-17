using System.Security.Cryptography;
using System.Text;
using GajinoAgencies.Settings;
using Microsoft.Extensions.Options;

namespace GajinoAgencies.Services;

public interface IPasswordManagerService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
    string GetSalt { get; }

    //===================================

    (string hashedPassword, string salt) HashPassword(string plainPassword);
    bool VerifyPassword(string plainPassword, string hashedPassword, string salt);

}

//##########################
//##########################
public class PasswordManagerService : IPasswordManagerService
{
    private readonly string _salt;
    private readonly int _saltSizeAsByte;

    //Ascii-code for slat string characters
    private readonly byte[] _byteSaltKey;

    public PasswordManagerService(IOptions<CryptographySettings> cryptographySettings)
    {
        _salt = cryptographySettings.Value.Salt;
        _saltSizeAsByte = cryptographySettings.Value.SaltSizeAsByte;
        _byteSaltKey = Encoding.UTF8.GetBytes(_salt);
        //_salt = GenerateSalt();
    }

    public string Encrypt(string plainText)
    {
        var initialVector = new byte[_saltSizeAsByte];
        byte[] encryptedBytes;

        using (var aes = Aes.Create())
        {
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _byteSaltKey;
            aes.IV = initialVector;
            var encryption = aes.CreateEncryptor(aes.Key, aes.IV);
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, encryption, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cryptoStream))
                    {
                        sw.Write(plainText);
                    }

                    encryptedBytes = ms.ToArray();
                }
            }
        }

        var encryptedText = Convert.ToBase64String(encryptedBytes);
        return encryptedText;
    }

    public string Decrypt(string encryptedText)
    {
        var initialVector = new byte[_saltSizeAsByte];
        var buffer = Convert.FromBase64String(encryptedText);
        using var aes = Aes.Create();
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _byteSaltKey;
        aes.IV = initialVector;
        var encryption = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(ms, encryption, CryptoStreamMode.Read);
        using var sr = new StreamReader(cryptoStream);
        var plainText = sr.ReadToEnd();
        return plainText;
    }

    public string GetSalt => _salt;

    private string GenerateSalt()
    {
        var salt = new byte[_saltSizeAsByte];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return Convert.ToBase64String(salt);
    }

    public (string hashedPassword, string salt) HashPassword(string plainPassword)
    {
        var salt = GenerateSalt();
        using var hmac = new HMACSHA256(Convert.FromBase64String(salt));
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword)));
        return (hash, salt);
    }

    public bool VerifyPassword(string plainPassword, string hashedPassword, string salt)
    {
        using var hmac = new HMACSHA256(Convert.FromBase64String(salt));
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(plainPassword)));
        return hash == hashedPassword;
    }
}


//https://ianvink.wordpress.com/2022/12/03/a-straightforward-way-in-c-net-to-encrypt-and-decrypt-a-string-using-aes/
//https://www.codeproject.com/Questions/379525/Padding-is-invalid-and-cannot-be-removed-Exception