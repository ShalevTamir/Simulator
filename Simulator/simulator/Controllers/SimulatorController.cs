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
        private readonly FrameCreatorService _frameCreatorService;

        public SimulatorController(SimulatorService simulatorService, FrameCreatorService frameCreatorService)
        {
            _simulatorService = simulatorService;
            _frameCreatorService = frameCreatorService;
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

        [HttpPost("apply-condition")]
        public ActionResult ApplyTeleParameterCondition([FromBody] TeleGenerationConditionDto condition)
        {
            var additionSuccessful = _frameCreatorService.addGenerationCondition(condition);
            if (additionSuccessful) return Ok();
            else return BadRequest("Condition with parameter " + condition.Name + "already exists");
        }

        [HttpPost("remove-condition")]
        public ActionResult RemoveParameterCondition([FromBody] string parameterName)
        {
            var removalSuccessful = _frameCreatorService.removeGenerationCondition(parameterName);
            if (removalSuccessful) return Ok();
            else return BadRequest("Condition with parameter " + parameterName + " doesn't exist");
        }
    }
}
