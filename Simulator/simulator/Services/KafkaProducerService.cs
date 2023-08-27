using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;

namespace Simulator.simulator.Services
{
    public class KafkaProducerService
    {
        private ProducerConfig _producerConfig;       
        private readonly string TOPIC_NAME ;
        private CancellationTokenSource _currentTokenSource;
        
        public KafkaProducerService(IConfiguration configuration)
        {
            TOPIC_NAME = configuration["KafkaProducer:TopicName"];
            string kafkaUrl = configuration["KafkaProducer:KafkaUrl"];
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaUrl
            };
           
        }
        public IProducer<Null,string> OpenProducerAsync(CancellationTokenSource tokenSource)
        {
           
            _currentTokenSource = tokenSource;
            return new ProducerBuilder<Null, string>(_producerConfig).SetErrorHandler(ProducerErrorCallback).Build();
        }
        public void Produce(IProducer<Null,string> producer, string messageToProduce)
        {
            producer.Produce(TOPIC_NAME, new Message<Null, string> { Value = messageToProduce });   
        }
        public Task<DeliveryResult<Null,string>> ProduceAsync(IProducer<Null, string> producer, string messageToProduce)
        {
            return producer.ProduceAsync(TOPIC_NAME, new Message<Null, string> { Value = messageToProduce },_currentTokenSource.Token);
        }
        private void ProducerErrorCallback(IProducer<Null, string> producer, Error error)
        {
            if(!_currentTokenSource.IsCancellationRequested)
                _currentTokenSource.Cancel();
        }
    }
}
