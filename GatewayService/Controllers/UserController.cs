using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using GatewayService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;

namespace GatewayService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(IHttpClientFactory httpClientFactory, HttpClient client)
        {
            _httpClientFactory = httpClientFactory;
        }

        // api/User/login
        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Login(UserLogin model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", new UserLoginModel(model.Login, model.Pass));

                // Check if the response status code is 2xx
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    var jwt = GenerateJwtToken(result.Id, model.RememberMe);
                    var userAndToken = new JWTAndUser() { Token = jwt, User = result };
                    return Ok(userAndToken);
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    return BadRequest(message);
                }
            }
        }       
        
        // api/User/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserUpdateModel model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                // Set the base address of the API you want to call
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", model);
                // Console.WriteLine(response.Content.ToString());
                // Console.WriteLine(response.StatusCode);

                // Check if the response status code is 2xx
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    using (var client2 = _httpClientFactory.CreateClient())
                    {
                        client2.BaseAddress = new Uri("http://localhost:5002/");
                        HttpResponseMessage response2 = await client2.PostAsJsonAsync($"api/Tasks/{result.Id}" , "");
                        if (response2.IsSuccessStatusCode)
                        {
                            return Ok(result);
                        }
                        else
                        {
                            return Conflict("Could not create user node");
                        }
                    }
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    return Conflict(message);
                }
            }
        }
        
        // api/User/update
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> Update(UserUpdateModel model)
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                
                // Set the base address of the API you want to call
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (userId == null) return Unauthorized();
                
                client.BaseAddress = new System.Uri("http://localhost:5001/");

                // Send a POST request to the login endpoint
                HttpResponseMessage response = await client.PutAsJsonAsync($"api/Users/{userId}", model);
                // Console.WriteLine(response.Content.ToString());
                // Console.WriteLine(response.StatusCode);

                // Check if the response status code is 2xx
                if (response.IsSuccessStatusCode)
                {
                    // You can deserialize the response content here if needed
                    var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return Ok(result);
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    return Conflict(message);
                }
            }
        }
        
        // api/User/delete
        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            // Create an HttpClient instance using the factory
            using (var client = _httpClientFactory.CreateClient())
            {
                
                // Set the base address of the API you want to call
                var userId = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
                if (userId == null) return Unauthorized();
                
                client.BaseAddress = new System.Uri("http://localhost:5002/");
                var response2 = await client.DeleteAsync($"api/Tasks/{userId}");
                
                using (var client2 = _httpClientFactory.CreateClient())
                {
                    client2.BaseAddress = new System.Uri("http://localhost:5001/");

                    // Send a POST request to the login endpoint
                    HttpResponseMessage response = await client2.DeleteAsync($"api/Users/{userId}");
                    // Console.WriteLine(response.Content.ToString());
                    // Console.WriteLine(response.StatusCode);

                    // Check if the response status code is 2xx
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        // You can deserialize the response content here if needed
                        var result = await response.Content.ReadAsStringAsync();
                        return Ok(result);
                    }
                    else
                    {
                        return Conflict("Could not delete user");
                    }
                }
            }
        }

        [Authorize]
        [HttpGet("jwt")]
        public ActionResult<string> Jwt()
        {
            var userName = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

            foreach (var claim in User.Claims)
            {
                Console.WriteLine(claim.Type + " " + claim.Value);
            }
            Console.WriteLine("jwt");
            return Ok($"Hello, {userName}");
        }
        


        private string GenerateJwtToken(string userId, bool stayLoggedIn = false)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", userId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("q.^KtTxmG[<Q%+*\"Lk?;6p{w3DP/B7bWU}M8:Ce$a5S(fV,@H~"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "ServiceWebApi",
                audience: "localhost:5000",
                claims: claims,
                expires: stayLoggedIn ? DateTime.Now.AddMonths(1) : DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
