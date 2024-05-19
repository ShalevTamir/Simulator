using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Simulator.simulator.Models.Dto;
using Simulator.simulator.Services;
using System.Diagnostics;
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
            Debug.WriteLine("CONSTRAINT " + condition.ID + " " + condition.Name + " " + condition.TopRestriction + " " + condition.BottomRestriction);
            var additionSuccessful = _frameCreatorService.AddGenerationCondition(condition);
            if (additionSuccessful) return Ok();
            else return BadRequest("Invalid parameter name or condition with ID " + condition.ID + " already exists");
        }

        [HttpPost("remove-condition")]
        public ActionResult RemoveParameterCondition([FromBody] int ID)
        {
            var removalSuccessful = _frameCreatorService.removeGenerationCondition(ID);
            if (removalSuccessful) return Ok();
            else return BadRequest("Condition with ID " + ID + " doesn't exist");
        }

        [HttpGet]
        public ActionResult GetAllConditions()
        {
            return Ok(JsonConvert.SerializeObject(
                _frameCreatorService.GetAllConditions()
                .Select((teleParameter) => teleParameter.ID)
                ));
        }
    }
}
