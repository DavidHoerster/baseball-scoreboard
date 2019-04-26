using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gbac_baseball.web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Dapper;

namespace gbac_baseball.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IConfiguration _config;
        public TeamController(IConfiguration config)
        {
            _config = config;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> Get()
        {
            using (var conn = new MySqlConnection(_config["Azure:MySql:ConnectionString"]))
            {
                var teams = await conn.QueryAsync<Team>("SELECT yearID, lgID, teamID, name FROM baseball2018.teams WHERE yearID = 2018 ORDER BY lgID, name");

                return Ok(teams);
            }
        }
    }
}
