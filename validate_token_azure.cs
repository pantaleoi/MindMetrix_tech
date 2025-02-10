using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

public static class ValidateToken
{
    private static readonly string SecretKey = "my_secret_key";

    [FunctionName("ValidateToken")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Received JWT validation request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        string token = JsonConvert.DeserializeObject<string>(requestBody);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey)),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out _);
            
            return new OkObjectResult(new { message = "Token Validated!", user = principal.Identity.Name });
        }
        catch (SecurityTokenException)
        {
            return new UnauthorizedResult();
        }
    }
}
