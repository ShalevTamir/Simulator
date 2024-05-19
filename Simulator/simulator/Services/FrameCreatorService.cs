using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Simulator.simulator.DTO;
using Simulator.simulator.Extentions;
using Simulator.simulator.Models;
using Simulator.simulator.Models.Dto;

namespace Simulator.simulator.Services
{
    public class FrameCreatorService
    {        

        private readonly CorrelatorService _correlatorService;
        private readonly JsonUtilsService _jsonUtilsService;
        private readonly IEnumerable<IcdParameter> _icdParameters;
        private readonly Random _random;
        private readonly List<TeleGenerationConditionDto> _teleGenerationConditions;
        public FrameCreatorService(CorrelatorService correlatorService, JsonUtilsService jsonUtilsService)
        {
            _correlatorService = correlatorService;
            _jsonUtilsService = jsonUtilsService;
            _icdParameters = _jsonUtilsService.DeserializeIcdFile();
            _random = new Random();
            _teleGenerationConditions = new List<TeleGenerationConditionDto>();
        }
        
        public TelemetryParameterDto GenerateTelemetryParam(IcdParameter icoParameter)
        {
            int minValue = int.Parse(icoParameter.MinValue);
            int maxValue = int.Parse(icoParameter.MaxValue);
            var conditions = GetConditions(icoParameter.ParameterName);
            foreach (var condition in conditions)
            {
                (minValue, maxValue) = condition.ApplyRestriction(minValue, maxValue);
                maxValue -= condition.TopRestriction == null ? 0 : 1;
            }
            int randomValue = minValue <= maxValue ? _random.Next(minValue, maxValue + 1) : 0;
            return new TelemetryParameterDto(icoParameter.ParameterName, randomValue.ToString(),icoParameter.Units);
        }
        public TelemetryFrameDTO ConvertIcdToFrame()
        {
            _correlatorService.GenerateCorrValue();
          
            List<TelemetryParameterDto> lstTeleParams = new List<TelemetryParameterDto>();
            foreach(var icdParameter in _icdParameters)
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
        private bool ConditionExists(int ID)
        {
            return _teleGenerationConditions.Any(value => value.ID == ID);
        }
        private TeleGenerationConditionDto[] GetConditions(string parameterName)
        {
            var filteredCollection = _teleGenerationConditions.Where((value) => string.Equals(value.Name, parameterName, StringComparison.OrdinalIgnoreCase));
            return filteredCollection.ToArray();
            //return filteredCollection.Any() ? filteredCollection.First() : null;
        }

        public bool AddGenerationCondition(TeleGenerationConditionDto condition)
        {
            if(ConditionExists(condition.ID) || !_icdParameters.Any(parameter => string.Equals(parameter.ParameterName, condition.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            else
            {
                _teleGenerationConditions.Add(condition);
                return true;
            }
        }

        public bool removeGenerationCondition(int ID)
        {
            if (!ConditionExists(ID))
            {
                return false;
            }
            else
            {
                _teleGenerationConditions.RemoveAll((condition) => condition.ID == ID);
                return true;
            }
        }

        public IEnumerable<TeleGenerationConditionDto> GetAllConditions()
        {
            return _teleGenerationConditions;
        }
    }
}
