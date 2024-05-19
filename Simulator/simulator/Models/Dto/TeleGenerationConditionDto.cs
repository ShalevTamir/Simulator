namespace Simulator.simulator.Models.Dto
{
    public class TeleGenerationConditionDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? BottomRestriction { get; set; }
        public int? TopRestriction { get; set; }
    }
}
