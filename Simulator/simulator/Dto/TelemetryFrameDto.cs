using System;
using System.Collections.Generic;
using Simulator.simulator.DTO;

namespace Simulator.simulator.Models
{
    public class TelemetryFrameDTO
    {
        public int FrameId { get; set; }
        public DateTime TimeStamp { get;  set; }
        public IEnumerable<TelemetryParameterDto> Parameters { get; set; }

        private static int amountOfFrames = 0;
        public TelemetryFrameDTO(IEnumerable<TelemetryParameterDto> lstParams)
        {
            amountOfFrames++;
            FrameId = amountOfFrames;
            TimeStamp = DateTime.Now;
            Parameters = lstParams;
        }
    }
}
