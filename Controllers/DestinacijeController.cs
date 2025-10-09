using Microsoft.AspNetCore.Mvc;
using Neo4jClient;
using putovanjeApp1.Models;


namespace putovanjeApp1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinacijaController : ControllerBase
    {
        private readonly IGraphClient _client;

        public DestinacijaController(IGraphClient client)
        {
            _client = client;
        }

        // 🟢 GET: api/destinacija
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var destinacije = await _client.Cypher
                .Match("(d:Destinacija)")
                .Return(d => d.As<Destinacija>())
                .ResultsAsync;

            return Ok(destinacije.ToList());
        }

        // 🟡 GET: api/destinacija/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var destinacija = await _client.Cypher
                .Match("(d:Destinacija)")
                .Where((Destinacija d) => d.id == id)
                .Return(d => d.As<Destinacija>())
                .ResultsAsync;

            var result = destinacija.SingleOrDefault();
            return result != null ? Ok(result) : NotFound($"Destinacija sa ID {id} nije pronađena.");
        }

        // 🔵 POST: api/destinacija
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Destinacija novaDestinacija)
        {
            await _client.Cypher
                .Create("(d:Destinacija $novaDestinacija)")
                .WithParam("novaDestinacija", novaDestinacija)
                .ExecuteWithoutResultsAsync();

            return Ok("Destinacija uspešno dodata.");
        }

        // 🟠 PUT: api/destinacija/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Destinacija izmenjenaDestinacija)
        {
            await _client.Cypher
                .Match("(d:Destinacija)")
                .Where((Destinacija d) => d.id == id)
                .Set("d = $izmenjenaDestinacija")
                .WithParam("izmenjenaDestinacija", izmenjenaDestinacija)
                .ExecuteWithoutResultsAsync();

            return Ok("Destinacija uspešno izmenjena.");
        }

        // 🔴 DELETE: api/destinacija/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _client.Cypher
                .Match("(d:Destinacija)")
                .Where((Destinacija d) => d.id == id)
                .DetachDelete("d")
                .ExecuteWithoutResultsAsync();

            return Ok("Destinacija uspešno obrisana.");
        }
    }
}

