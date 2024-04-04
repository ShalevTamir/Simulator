﻿using System;
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
            var condition = GetCondition(icoParameter.ParameterName);
            if(condition != null)
            {
               (minValue, maxValue) = condition.ApplyRestriction(minValue, maxValue);
                //If top restriction exits, generate 1 bellow it, don't reach the restriction
                maxValue -= condition.TopRestriction == null ? 0 : 1;
            }
            int randomValue = _random.Next(minValue, maxValue + 1);
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
        private bool ConditionExists(string parameterName)
        {
            return GetCondition(parameterName) != null;
        }

        private TeleGenerationConditionDto? GetCondition(string parameterName)
        {
            var filteredCollection = _teleGenerationConditions.Where((value) => value.Name == parameterName);
            return filteredCollection.Any() ? filteredCollection.First() : null;
        }

        public bool AddGenerationCondition(TeleGenerationConditionDto condition)
        {
            if(ConditionExists(condition.Name) || !_icdParameters.Any(parameter => parameter.ParameterName == condition.Name))
            {
                return false;
            }
            else
            {
                _teleGenerationConditions.Add(condition);
                return true;
            }
        }

        public bool removeGenerationCondition(string parameterName)
        {
            if (!ConditionExists(parameterName))
            {
                return false;
            }
            else
            {
                _teleGenerationConditions.RemoveAll((condition) => condition.Name == parameterName);
                return true;
            }
        }

        public IEnumerable<TeleGenerationConditionDto> GetAllConditions()
        {
            return _teleGenerationConditions;
        }
    }
}
