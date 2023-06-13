using System.Security.Cryptography;
using System.Text;

namespace ELearnApp.Extentions;

public static class StringExtension
{
    private static readonly SHA256 HashProvider =  SHA256.Create();
    private static UTF8Encoding Encoding = new();
    
    
    public static async Task<string> ComputeHash(this string s)
    {
  
            using var stream = new MemoryStream(Encoding.GetBytes(s));
            var hash = await HashProvider.ComputeHashAsync(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}