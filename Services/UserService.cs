using Neo4jClient;
using Neo4jClient.Cypher;
using putovanjeApp1.Dtos;
using putovanjeApp1.Models;

namespace putovanjeApp1.Services
{
    public class UserService
    {
        private readonly IGraphClient _client;

        public UserService(IGraphClient client)
        {
            _client = client;
        }

        // Registracija korisnika
        public async Task<User> RegisterAsync(User user)
        {
            user.guid = Guid.NewGuid();

            await _client.Cypher
                .Create("(u:User $user)")
                .WithParam("user", new
                 {
                     Guid = user.guid.ToString(),//sto to.string? i jel treba mala slova
                    Ime = user.ime,
                    Email = user.email,
                    PasswordHash = user.passwordHash,
                    Interesovanja = user.interesovanja
     })
     .ExecuteWithoutResultsAsync();


            return user;
        }

        // Login - vraća Guid korisnika ako postoji
        public async Task<Guid?> LoginAsync(string email, string password)
        {
            var result = await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.email == email && u.passwordHash == password)
                .Return(u => u.As<User>().guid)
                .ResultsAsync;

            return result.FirstOrDefault();
        }

        public async Task<User?> GetByGuidAsync(Guid guid)
        {
            var results = await _client.Cypher
                .Match("(u:User)")
                .Where("u.Guid = $guid")
                .WithParam("guid", guid.ToString())
                .Return(u => u.As<User>())
                .ResultsAsync;

            return results.FirstOrDefault();
        }

        public async Task<bool> UpdateAsync(Guid guid, UserDto updateDto)
        {
            // Ne mapiramo passwordHash ovde; za promenu lozinke imaj poseban endpoint
            var props = new
            {
                Ime = updateDto.ime,
                Email = updateDto.email,
                Interesovanja = updateDto.interesovanja
            };

                 await _client.Cypher
                .Match("(u:User)")
                .Where("u.Guid = $guid")
                .WithParam("guid", guid.ToString())
                .Set("u.Ime = $Ime, u.Email = $Email, u.Interesovanja = $Interesovanja")
                .WithParams(props)
                .ExecuteWithoutResultsAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid guid)
        {
            await _client.Cypher
                .Match("(u:User)")
                .Where("u.Guid = $guid")
                .WithParam("guid", guid.ToString())
                .DetachDelete("u")
                .ExecuteWithoutResultsAsync();

            return true;
        }




        // preporuka destinacija po interesovanjima korisnika
        public async Task<List<Destinacija>> GetRecommendedDestinations(Guid userId)
        {
            var destinations = await _client.Cypher
                .Match("(u:User)-[:LIKES]->(a:Aktivnost)<-[:NUDI]-(d:Destinacija)")
                .Where((User u) => u.guid == userId)
                .Return(d => d.As<Destinacija>())
                .ResultsAsync;

            return destinations.ToList();
        }

        // preporuka atrakcija ili aktivnosti korisniku
        public async Task<List<Atrakcija>> GetRecommendedActivities(Guid userId)
        {
            var activities = await _client.Cypher
                .Match("(u:User)-[:VISITED]->(p:Putovanje)-[:SADRZI]->(d:Destinacija)-[:NUDI]->(at:Atrakcija)")
                .Where((User u) => u.guid == userId)
                .With("u, at, d")
                .Match("(other:User)-[:VISITED]->(:Putovanje)-[:SADRZI]->(d)-[:NUDI]->(at)")
                .Where("other.Id <> $userId")
                .WithParam("userId", userId)
                .Return(at => at.As<Atrakcija>())
                .ResultsAsync;

            return activities.Distinct().ToList();
        }

        public async Task<List<Destinacija>> GetRecommendedDestinationsAsync(Guid userId, int limit = 10)
        {
            // 1️⃣ Pronađi slične korisnike po zajedničkim aktivnostima i putovanjima
            var similarUsers = await _client.Cypher
                .Match("(u:User {Id: $userId})-[:LIKES|PUTOVAO_NA]->(x)<-[:LIKES|PUTOVAO_NA]-(other:User)")
                .WithParam("userId", userId)
                .ReturnDistinct(other => other.As<User>().guid)
                .ResultsAsync;

            var similarUserIds = similarUsers.ToList();

            if (!similarUserIds.Any())
                return new List<Destinacija>(); // nema sličnih korisnika

            // 2️⃣ Pronađi destinacije koje slični korisnici posećuju, a korisnik nije
            var recommended = await _client.Cypher
                .Match("(other:User)-[:PUTOVAO_NA]->(d:Destinacija)")
                .Where("other.Id IN $similarUserIds")
                .AndWhere("(NOT (:User {Id: $userId})-[:PUTOVAO_NA]->(d))")
                .WithParam("similarUserIds", similarUserIds)
                .WithParam("userId", userId)
                .Return((d, other) => new
                {
                    Destinacija = d.As<Destinacija>(),
                    Popularity = "count(other)" // ✅ ručno zadat COUNT
                })
                .OrderByDescending("count(other)")
                .Limit(limit)
                .ResultsAsync;

            // 3️⃣ Vraćamo samo listu destinacija
            return recommended.Select(r => r.Destinacija).ToList();
        }








    }
}
