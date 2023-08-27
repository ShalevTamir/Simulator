using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using Simulator.simulator.Models.Enums;

namespace Simulator.simulator.Services
{
    public class SimulatorService
    {
        public int DelayInSeconds { get; set; }

        private SimulatorState _state;
        private CancellationTokenSource _currentTokenSource;
        private FrameCreatorService _frameCreatorService;
        private KafkaProducerService _kafkaProducerService;
        public SimulatorService(FrameCreatorService frameService, KafkaProducerService kafkaService)
        {
            _state = SimulatorState.STOP;
            DelayInSeconds = 1;
            _frameCreatorService = frameService;
            _kafkaProducerService = kafkaService;
        }

        //true - if changed successfully
        //false - if current state equals to given state
        public bool ChangeState(SimulatorState state)
        {            
            if (_state == state)
                return false;

            switch (state)
            {
                case SimulatorState.START:
                    StartGenerating();
                    break;

                case SimulatorState.STOP:
                    StopGenerating();
                    break;
            }
            _state = state;
            return true;
        }


        public void Restart()
        {
            StopGenerating();
            StartGenerating();
        }

        public bool isRunning()
        {
            return _state == SimulatorState.START;
        }

        private void StartGenerating()
        {
            SetupToken();
            Task.Factory.StartNew(StartGeneratingLogic, _currentTokenSource.Token);
        }
        private void SetupToken()
        {
            _currentTokenSource = new CancellationTokenSource();
        }

        private async Task StartGeneratingLogic()
        {
            try
            {
                using (IProducer<Null,string> producer = _kafkaProducerService.OpenProducerAsync(_currentTokenSource))
                {
                    while (!_currentTokenSource.Token.IsCancellationRequested)
                    {
                        var telemetryFrame = _frameCreatorService.ConvertIcdToFrame();
                        var formattedFrame = JsonConvert.SerializeObject(telemetryFrame,Formatting.Indented);
                        await _kafkaProducerService.ProduceAsync(producer, formattedFrame);
                        Debug.WriteLine($"{formattedFrame} \n, length {telemetryFrame.Parameters.Count()}");
                        await Task.Delay(DelayInSeconds * 1000, _currentTokenSource.Token);
                    }                    
                }
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("Task stopped");
            }
           
        }

         private void StopGenerating()
        {
            if (_state == SimulatorState.START)            
                _currentTokenSource.Cancel();
            
        }
    }
}
