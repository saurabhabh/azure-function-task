using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace azure_function
{
    public static class Login
    {
        private const string SECRET_KEY = "THIS_IS_A_SECRET_KEY_WHICH_SHOULD_BE_LONG_ENOUGH"; // Replace with a secure key
        private static readonly SymmetricSecurityKey SigningKey = new(Encoding.UTF8.GetBytes(SECRET_KEY));

        // Static list of users (username and password)
        private static readonly List<User> Users = new()
        {
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "user", Password = "user123", Role = "User" },
        };

        [FunctionName("Login")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing Login request.");

            string requestBody = new StreamReader(req.Body).ReadToEndAsync().Result;
            var data = JsonSerializer.Deserialize<LoginRequest>(requestBody);
            string username = data?.Username;
            string password = data?.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return new BadRequestObjectResult("Username and password are required.");
            }

            // Check if the user exists
            var user = Users.Find(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                return new UnauthorizedObjectResult("Invalid username or password.");
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new OkObjectResult(new { token = tokenHandler.WriteToken(token) });
        }

        // Model classes
        private class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
        }

        private class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
