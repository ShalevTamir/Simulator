using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Simulator.simulator.Models.Dto;
using Simulator.simulator.Services;
using System.Linq;

namespace Simulator.simulator.Controllers
{
    [Route("tele-conditions")]
    [ApiController]
    public class TeleConditionsController: Controller
    {
        private readonly FrameCreatorService _frameCreatorService;

        public TeleConditionsController(FrameCreatorService frameCreatorService)
        {
            _frameCreatorService = frameCreatorService;
        }

        [HttpPost("apply-condition")]
        public ActionResult ApplyTeleParameterCondition([FromBody] TeleGenerationConditionDto condition)
        {
            var additionSuccessful = _frameCreatorService.AddGenerationCondition(condition);
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

        [HttpGet]
        public ActionResult GetAllConditions()
        {
            return Ok(JsonConvert.SerializeObject(
                _frameCreatorService.GetAllConditions()
                .Select((teleParameter) => teleParameter.Name)
                ));
        }
    }
}
