using Domain.Common;
using System;

namespace Domain.Entities{
    public class BusinessMessageSettings : AuditableEntity
    {
        public Guid Id { get; set; }
        public string WebhookUrl { get; set; }
        public string ApiKey {get; set;}
        public string BotName { get; set; }
        public string BotDescription { get; set; }
        public int TestCounter {get; set;}
       
        public bool IsTest { get; set; }
        /// <summary>
        /// If apikey is regstered and business hasn't been registred to
        /// "WABABaseUrl/v1/configs/webhook", the a bgTask picks up and congfigures..
        /// </summary>
        public bool IsWebhookConfigured { get; set; }
        public Guid BusinessId { get; set; }
        public Business Business { get; set; }
    }
}