using Neo4jClient;
using Neo4jClient.Cypher;
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

        // Preporuka destinacija po interesovanjima korisnika
        public async Task<List<Destinacija>> GetRecommendedDestinations(int userId)
        {
            var destinations = await _client.Cypher
                .Match("(u:User)-[:LIKES]->(a:Aktivnost)<-[:NUDI]-(d:Destinacija)")
                .Where((User u) => u.id == userId)
                .Return(d => d.As<Destinacija>())
                .ResultsAsync;

            return destinations.ToList();
        }

        // Preporuka atrakcija ili aktivnosti korisniku
        public async Task<List<Atrakcija>> GetRecommendedActivities(int userId)
        {
            var activities = await _client.Cypher
                .Match("(u:User)-[:VISITED]->(p:Putovanje)-[:SADRZI]->(d:Destinacija)-[:NUDI]->(at:Atrakcija)")
                .Where((User u) => u.id == userId)
                .With("u, at, d")
                .Match("(other:User)-[:VISITED]->(:Putovanje)-[:SADRZI]->(d)-[:NUDI]->(at)")
                .Where("other.Id <> $userId")
                .WithParam("userId", userId)
                .Return(at => at.As<Atrakcija>())
                .ResultsAsync;

            return activities.Distinct().ToList();
        }

        public async Task<List<Destinacija>> GetRecommendedDestinationsAsync(int userId, int limit = 10)
        {
            // 1️⃣ Pronađi slične korisnike po zajedničkim aktivnostima i putovanjima
            var similarUsers = await _client.Cypher
                .Match("(u:User {Id: $userId})-[:LIKES|PUTOVAO_NA]->(x)<-[:LIKES|PUTOVAO_NA]-(other:User)")
                .WithParam("userId", userId)
                .ReturnDistinct(other => other.As<User>().id)
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
