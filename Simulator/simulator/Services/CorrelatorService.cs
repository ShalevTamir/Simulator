using Microsoft.Extensions.Configuration;
using Simulator.simulator.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Simulator.simulator.Services
{
    public class CorrelatorService
    {
        public readonly string CORRELATOR_ICD_NAME;
        private readonly int _minCorrValue;
        private readonly int _maxCorrValue;
        private readonly Random _random;
        public CorrelatorService(JsonUtilsService jsonUtilsService, IConfiguration configuration)
        {
            var icdList = jsonUtilsService.DeserializeIcdFile();
            CORRELATOR_ICD_NAME = configuration["Correlator:CorrelatorIcdName"];          
            IcdParameter correlatorIcd = FindCorrelatorIcd(icdList);
            _minCorrValue = int.Parse(correlatorIcd.MinValue);
            _maxCorrValue = int.Parse(correlatorIcd.MaxValue);
            _random = new Random();
        }
        private IcdParameter FindCorrelatorIcd(IEnumerable<IcdParameter> icdList)
        {
            var correlatorIcd = icdList.Where(icdParameter => icdParameter.ParameterName == CORRELATOR_ICD_NAME);
            if (correlatorIcd.Count() == 0) throw new FormatException($"Correlator was not found. Looked for correlator name: \"{CORRELATOR_ICD_NAME}\"");
            return correlatorIcd.First();
        }
        public void GenerateCorrValue()
        {            
            CorrValue = _random.Next(_minCorrValue,_maxCorrValue+1);
        }
        public int CorrValue { get; private set; }
    }
}
