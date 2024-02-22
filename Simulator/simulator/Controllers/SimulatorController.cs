using Microsoft.AspNetCore.Mvc;
using Simulator.simulator.Models.Dto;
using Simulator.simulator.Models.Enums;
using Simulator.simulator.Services;

namespace Simulator.simulator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SimulatorController : Controller
    {
        private readonly SimulatorService _simulatorService;

        public SimulatorController(SimulatorService simulatorService)
        {
            _simulatorService = simulatorService;
        }


        [HttpPut("state")]
        public ActionResult SetSimulatorState([FromBody] SimulatorState stateToSwitchTo)
        {
            bool success = _simulatorService.ChangeState(stateToSwitchTo);
            if (success) return Ok();
            else return BadRequest("Server is already in state " + stateToSwitchTo);
        }

        [HttpPut("delay")]
        public ActionResult SetDelay([FromBody] int seconds)
        {
            _simulatorService.DelayInSeconds = seconds;
            if (_simulatorService.isRunning())
                _simulatorService.Restart();
            return Ok();
        }
    }
}
