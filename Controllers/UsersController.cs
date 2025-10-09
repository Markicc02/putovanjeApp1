using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using Neo4jClient.Cypher;
using putovanjeApp1.Models;
using putovanjeApp1.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace putovanjeApp1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IGraphClient _client;
        private readonly UserService _userService; // 👈 dodato

        public UserController(IGraphClient client, UserService userService) // 👈 dodato
        {
            _client = client;
            _userService = userService;
        }


        // 🟢 GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _client.Cypher
                .Match("(u:User)")
                .Return(u => u.As<User>())
                .ResultsAsync;

            return Ok(users.ToList());
        }

        // 🟡 GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var users = await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.id == id)
                .Return(u => u.As<User>())
                .ResultsAsync;

            var result = users.SingleOrDefault();
            return result != null ? Ok(result) : NotFound($"User sa ID {id} nije pronađen.");
        }

        // 🔵 POST: api/user
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User noviUser)
        {
            await _client.Cypher
                .Create("(u:User $noviUser)")
                .WithParam("noviUser", noviUser)
                .ExecuteWithoutResultsAsync();

            return Ok("User uspešno dodat.");
        }

        // 🟠 PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User izmenjeniUser)
        {
            await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.id == id)
                .Set("u = $izmenjeniUser")
                .WithParam("izmenjeniUser", izmenjeniUser)
                .ExecuteWithoutResultsAsync();

            return Ok("User uspešno izmenjen.");
        }

        // 🔴 DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.id == id)
                .DetachDelete("u")
                .ExecuteWithoutResultsAsync();

            return Ok("User uspešno obrisan.");
        }

        // GET: api/user/{id}/recommendations/destinations
        [HttpGet("{id}/recommendations/destinations")]
        public async Task<IActionResult> GetRecommendedDestinations(int id)
        {
            var preporuke = await _client.Cypher
                .Match("(u:User)-[:LIKES]->(a:Aktivnost)<-[:NUDI]-(at:Atrakcija)<-[:SADRZI]-(d:Destinacija)")
                .Where((User u) => u.id == id)
                .Return(d => d.As<Destinacija>())
                .ResultsAsync;

            var result = preporuke.Distinct().ToList();
            return result.Any() ? Ok(result) : NotFound($"Nema preporučenih destinacija za korisnika sa ID {id}.");
        }

        // GET: api/user/{id}/recommendations/activities
        [HttpGet("{id}/recommendations/activities")]
        public async Task<IActionResult> GetRecommendedActivities(int id)
        {
            var preporuke = await _client.Cypher
                .Match("(u:User)-[:LIKES]->(a:Aktivnost)<-[:NUDI]-(at:Atrakcija)<-[:SADRZI]-(d:Destinacija)")
                .Where((User u) => u.id == id)
                .OptionalMatch("(u)-[:BEO_NA]->(p:Putovanje)-[:SADRZI]->(d2:Destinacija)-[:NUDI]->(at2:Atrakcija)")
                .With("collect(at2) as posetio, at, d")
                .Where("NOT at IN posetio")
                .Return(at => at.As<Aktivnost>())
                .ResultsAsync;

            var result = preporuke.Distinct().ToList();
            return result.Any() ? Ok(result) : NotFound($"Nema preporučenih aktivnosti za korisnika sa ID {id}.");
        }

        [HttpGet("{id}/similar")]
        public async Task<IActionResult> GetSimilarUsers(int id)
        {
            var similarUsers = await _client.Cypher
                .Match("(u:User {Id: $userId})-[:LIKES|PUTOVAO_NA]->(x)")
                .Match("(other:User)-[:LIKES|PUTOVAO_NA]->(x)")
                .Where("other.Id <> $userId")
                .WithParam("userId", id)
                .Return((other, x) => new {
                    User = other.As<User>(),
                    CommonCount = Return.As<int>("COUNT(x)")
                })
                .OrderByDescending("CommonCount")
                .Limit(10)
                .ResultsAsync;

            return Ok(similarUsers);
        }



        [HttpGet("{id}/recommendations/destinations2")]
        public async Task<IActionResult> GetRecommendedDestinations2(int id)
        {
            var preporuke = await _userService.GetRecommendedDestinationsAsync(id);
            return Ok(preporuke);
        }





    }
}
