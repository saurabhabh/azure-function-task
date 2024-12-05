using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace azure_function
{
    public static class Tasks
    {
        private const string SECRET_KEY = "THIS_IS_A_SECRET_KEY_WHICH_SHOULD_BE_LONG_ENOUGH"; // Same key as in Login

        [FunctionName("Tasks")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing Tasks request.");

            string authHeader = req.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new UnauthorizedObjectResult("Authorization token is missing.");
            }

            string token = authHeader.Substring("Bearer ".Length);

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    throw new Exception("Invalid token.");
                }

                string role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                // Define task data using TaskItem class
                var tasks = new List<TaskItem>
                {
                    new TaskItem { Id = 1, Task = "Admin Task 1", Role = "Admin" },
                    new TaskItem { Id = 2, Task = "Admin Task 2", Role = "Admin" },
                    new TaskItem { Id = 3, Task = "User Task 1", Role = "User" },
                    new TaskItem { Id = 4, Task = "User Task 2", Role = "User" }
                };

                var userTasks = new List<TaskItem>();

                if(role == "Admin")
                {
                    userTasks = tasks;
                }
                else
                {
                    userTasks = tasks.Where(t => t.Role == role).ToList();
                }
                    

                return new OkObjectResult(userTasks);
            }
            catch (Exception ex)
            {
                log.LogError($"Token validation failed: {ex.Message}");
                return new UnauthorizedObjectResult("Invalid or expired token.");
            }
        }
    }

    // TaskItem class definition
    public class TaskItem
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public string Role { get; set; }
    }
}
