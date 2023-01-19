using Domain.Entities;
using Domain.Entities.FormProcessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public interface IDuplicateFIlterHelper : IAutoDependencyService
    {
        Task<Dictionary<string, TResponse>> AddAndRetrieve<TResponse, TRequest>(List<TRequest> request, Dictionary<string, TRequest> keyValuePairs);
        Task<(string key, IDictionary<string, TResponse> dict)> Get<TResponse, TRequest>(TRequest item, Dictionary<string, TRequest> request);
        void Remove<T>(string key, Dictionary<string, T> keyValuePairs);
    }

    public class DuplicateFIlterHelper : IDuplicateFIlterHelper
    {
        /*Todo: Write a function to Add to dictionary.*/
        public async Task<Dictionary<string, TResponse>> AddAndRetrieve<TResponse, TRequest>(List<TRequest> request, Dictionary<string, TRequest> keyValuePairs)
        {
            foreach (var item in request)
            {
                var getInboundDict = await Get<TResponse, TRequest>(item,keyValuePairs);
                if (getInboundDict.dict?.Values is null)
                {
                    keyValuePairs.Add(getInboundDict.key, item);
                }
            }
            return await Task.FromResult(keyValuePairs as Dictionary<string, TResponse>);
        }

        /*Todo: Write a Synchronous function to check if key already exist in a diction of values..*/
        public async Task<(string key, IDictionary<string, TResponse> dict)> Get<TResponse, TRequest>(TRequest item,Dictionary<string, TRequest> request)
        {
            string key = string.Empty;
            if(item.GetType() == typeof(InboundMessage))
            {
                var inbound = item as InboundMessage;
                key = inbound.Wa_Id;
            }
            else if(item.GetType() == typeof(FormRequestResponse))
            {
                var inbound = item as FormRequestResponse;
                key = inbound.To;
            }

            if (request !=null && request.ContainsKey(key))
            {
               return await Task.FromResult((key, request as Dictionary<string, TResponse>)) ;
            }
            return (key, null);
        }

        public void Remove<T>(string key, Dictionary<string, T> keyValuePairs)
        {
            keyValuePairs.Remove(key);
        }
    }
}
