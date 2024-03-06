using Microsoft.Extensions.Hosting;
using Simulator.simulator.Models.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator.simulator.Services
{
    public class StartupService : IHostedService
    {
        SimulatorService _simulatorService;
        public StartupService(SimulatorService simulatorService) 
        { 
            _simulatorService = simulatorService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _simulatorService.ChangeState(SimulatorState.START);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
