using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace System
{
    public static class AzureExtensions
    {
        private static QueueClient _queueClient;
        private static string _queueName, _nameSpace, _issuerName, _issuerKey;
        public static Uri GetAddress(string nameSpace)
        {
            return ServiceBusEnvironment.CreateServiceUri("sb", nameSpace, String.Empty);
        }
        public static TokenProvider GetToken(string issuerName, string issuerKey)
        {
            return TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerKey);
        }
        public static NamespaceManager GetNameSpaceManager(string nameSpace, string issuerName, string issuerKey)
        {
            var uri = GetAddress(nameSpace);
            var tP = GetToken(issuerName, issuerKey);
            return new NamespaceManager(uri, tP);
        }
        public static QueueDescription GetQueue(string queueName, string nameSpace, string issuerName, string issuerKey)
        {
            NamespaceManager namespaceManager = GetNameSpaceManager(nameSpace, issuerName, issuerKey);
            return namespaceManager.GetQueue(queueName);
        }

        public static void AddToAzureQueue(this object o, string queueName, string nameSpace, string issuerName, string issuerKey)
        {
            if (_queueClient == null || queueName.ToLower() != _queueName || nameSpace.ToLower() != _nameSpace || issuerName.ToLower() != _issuerName || issuerKey.ToLower() != _issuerKey)
            {
                _queueName = queueName.ToLower();
                _nameSpace = nameSpace.ToLower();
                _issuerName = issuerName.ToLower();
                _issuerKey = issuerKey.ToLower();


                ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Http;
                System.Net.ServicePointManager.DefaultConnectionLimit = int.MaxValue;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.UseNagleAlgorithm = false;

                var credentials = GetToken(issuerName, issuerKey);

                // Get a client to the queue
                var messagingFactory = MessagingFactory.Create(GetAddress(nameSpace), credentials);
                _queueClient = messagingFactory.CreateQueueClient(queueName);
            }

            BrokeredMessage message = new BrokeredMessage(o);

            _queueClient.Send(message);
        }

    }
}
