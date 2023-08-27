using System;
using System.Collections.Generic;
using System.Diagnostics;
using Simulator.simulator.DTO;
using Simulator.simulator.Models;

namespace Simulator.simulator.Services
{
    public class FrameCreatorService
    {        

        private readonly CorrelatorService _correlatorService;
        private readonly JsonUtilsService _jsonUtilsService;
        private readonly Random _random;
        public FrameCreatorService(CorrelatorService correlatorService, JsonUtilsService jsonUtilsService)
        {
            _correlatorService = correlatorService;
            _jsonUtilsService = jsonUtilsService;
            _random = new Random();
        }
        
        public TelemetryParameterDto GenerateTelemetryParam(IcdParameter icoParameter)
        {
            int randomValue = _random.Next(int.Parse(icoParameter.MinValue), int.Parse(icoParameter.MaxValue) + 1);
            return new TelemetryParameterDto(icoParameter.ParameterName, randomValue.ToString(),icoParameter.Units);
        }
        public TelemetryFrameDTO ConvertIcdToFrame()
        {
            _correlatorService.GenerateCorrValue();
            IEnumerable<IcdParameter> icdParameters =  _jsonUtilsService.DeserializeIcdFile();
          
            List<TelemetryParameterDto> lstTeleParams = new List<TelemetryParameterDto>();
            foreach(var icdParameter in icdParameters)
            {   
                if (icdParameter.ParameterName != _correlatorService.CORRELATOR_ICD_NAME && (icdParameter.CorrValue == ""  || 
                                                           int.Parse(icdParameter.CorrValue) == _correlatorService.CorrValue))
                {
                  lstTeleParams.Add(GenerateTelemetryParam(icdParameter));                
                }
            }
            Debug.WriteLine($"correlator value {_correlatorService.CorrValue}");
            return new TelemetryFrameDTO(lstTeleParams);
        }
    }
}
