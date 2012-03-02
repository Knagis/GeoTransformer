/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FetchStatistics
{
    internal static class ServiceProxy
    {
        public static StatisticsService.ServiceClient CreateServiceClient()
        {
            var binding = new System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.None);
            var endpoint = new System.ServiceModel.EndpointAddress("http://geostats.miga.lv/Service.svc");
            return new StatisticsService.ServiceClient(binding, endpoint);
        }
    }
}

namespace FetchStatistics.StatisticsService
{
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "StatisticsService.Service")]
    public interface Service
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/Service/RetrieveWork", ReplyAction = "http://tempuri.org/Service/RetrieveWorkResponse")]
        System.Guid[] RetrieveWork();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/Service/SubmitWork", ReplyAction = "http://tempuri.org/Service/SubmitWorkResponse")]
        void SubmitWork(FetchStatistics.StatisticsData data);
    }

    public partial class ServiceClient : System.ServiceModel.ClientBase<FetchStatistics.StatisticsService.Service>, FetchStatistics.StatisticsService.Service
    {
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        public System.Guid[] RetrieveWork()
        {
            return base.Channel.RetrieveWork();
        }

        public void SubmitWork(FetchStatistics.StatisticsData data)
        {
            base.Channel.SubmitWork(data);
        }
    }
}