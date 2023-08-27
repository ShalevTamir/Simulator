namespace Simulator.simulator.DTO
{
    public class TelemetryParameterDto
    {
        public string Name;
        public string Value;
        public string Units;
        public TelemetryParameterDto(string name, string value, string units)
        {
            Name = name;
            Value = value;
            Units = units;
        }
    }
}
