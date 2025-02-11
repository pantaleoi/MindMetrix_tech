using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.Drawing;

class Program
{
    static void Main()
    {
        string secretKey = "my_secret_key";
        string userId = "12345";
        string sessionId = "session_67890";

        string token = GenerateJwtToken(secretKey, userId, sessionId);
        Console.WriteLine("Generated JWT Token:");
        Console.WriteLine(token);

        GenerateQRCode(token, "auth_qr_code.png");
        Console.WriteLine("QR Code saved as auth_qr_code.png");
    }

    public static string GenerateJwtToken(string secretKey, string userId, string sessionId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("user_id", userId),
            new Claim("session_id", sessionId),
            new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddSeconds(30).ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static void GenerateQRCode(string token, string filePath)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        {
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(token, QRCodeGenerator.ECCLevel.Q))
            {
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap bitmap = qrCode.GetGraphic(20))
                    {
                        bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }
    }
}
