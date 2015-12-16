namespace NServiceBus.Hosting.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NServiceBus.ConsistencyGuarantees;
    using NServiceBus.Extensibility;
    using NServiceBus.Settings;
    using NUnit.Framework;
    using Transports;

    [TestFixture]
    public class With_transport_tests
    {
        [Test]
        public void Should_configure_requested_transport()
        {
            var builder = new BusConfiguration();

            builder.EndpointName("myTests");
            builder.UseTransport<MyTestTransport>();

            Assert.IsInstanceOf<MyTestTransport>(builder.Settings.Get<TransportDefinition>());
        }

        [Test]
        public void Should_default_to_msmq_if_no_other_transport_is_configured()
        {
            var builder = new BusConfiguration();
            builder.EndpointName("myTests");

            Assert.True(builder.Settings.Get<TransportDefinition>() is MsmqTransport);
        }
    }

    public class MyTestTransportSender : IDispatchMessages
    {
 public Task Dispatch(IEnumerable<TransportOperation> outgoingMessages, ContextBag context)
        {
            throw new NotImplementedException();
        }
    }

    public class ConfigWithCustomTransport : IConfigureThisEndpoint, AsA_Server, UsingTransport<MyTestTransport>
    {
        public void Customize(BusConfiguration configuration)
        {
        }
    }
    class SecondConfigureThisEndpoint : IConfigureThisEndpoint
    {
        public void Customize(BusConfiguration configuration)
        {
        }
    }

    public class MyTestTransport : TransportDefinition
    {


        protected internal override TransportReceivingConfigurationResult ConfigureForReceiving(TransportReceivingConfigurationContext context)
        {
            return new TransportReceivingConfigurationResult(() => new FakeReceiver(context.Settings.Get<Exception>()), () => new FakeQueueCreator(), () => Task.FromResult(StartupCheckResult.Success));
        }

        protected internal override TransportSendingConfigurationResult ConfigureForSending(TransportSendingConfigurationContext context)
        {
            return new TransportSendingConfigurationResult(() => new FakeDispatcher(), () => Task.FromResult(StartupCheckResult.Success));
        }

        public override IEnumerable<Type> GetSupportedDeliveryConstraints()
        {
            return new List<Type>();
        }

        public override TransportTransactionMode GetSupportedTransactionMode()
        {
            return TransportTransactionMode.None;
        }

        public override IManageSubscriptions GetSubscriptionManager()
        {
            throw new NotImplementedException();
        }

        public override string GetDiscriminatorForThisEndpointInstance(ReadOnlySettings settings)
        {
            return null;
        }

        public override string ToTransportAddress(LogicalAddress logicalAddress)
        {
            return logicalAddress.ToString();
        }

        public override OutboundRoutingPolicy GetOutboundRoutingPolicy(ReadOnlySettings settings)
        {
            return new OutboundRoutingPolicy(OutboundRoutingType.Unicast, OutboundRoutingType.Unicast, OutboundRoutingType.Unicast);
        }

        public override string ExampleConnectionStringForErrorMessage => null;
    }
}