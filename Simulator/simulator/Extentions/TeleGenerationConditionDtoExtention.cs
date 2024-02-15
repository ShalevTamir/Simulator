using Simulator.simulator.Models.Dto;
using System;

namespace Simulator.simulator.Extentions
{
    public static class TeleGenerationConditionDtoExtention
    {
        public static (int, int) ApplyRestriction(this TeleGenerationConditionDto condition, int minValue, int maxValue)
        {
            return (
                condition.BottomRestriction == null ? minValue : (int)condition.BottomRestriction,
                condition.TopRestriction == null ? maxValue : (int)condition.TopRestriction
                );
        }
    }
}
