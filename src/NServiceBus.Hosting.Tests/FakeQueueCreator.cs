namespace NServiceBus.Hosting.Tests
{
    using System.Threading.Tasks;
    using NServiceBus.Transports;

    class FakeQueueCreator : ICreateQueues
    {
        public Task CreateQueueIfNecessary(QueueBindings queueBindings, string identity)
        {
            return Task.FromResult(0);
        }
    }
}