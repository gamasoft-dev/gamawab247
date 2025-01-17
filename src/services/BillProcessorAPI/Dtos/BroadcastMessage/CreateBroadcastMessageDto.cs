﻿using BillProcessorAPI.Enums;

namespace BillProcessorAPI.Dtos.BroadcastMessage
{

    public class BroadcastMessageDto
    {
        public Guid Id { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class CreateBroadcastMessageDto
    {
        public string To { get; set; }
        public string From { get; set; }
        public string ApiKey { get; set; }
        public string Message { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }

    }
}
