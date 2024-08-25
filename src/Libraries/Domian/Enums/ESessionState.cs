using System;
namespace Domain.Enums
{
    public enum ESessionState : int
    {
        PLAINCONVERSATION = 1,
        FORMCONVOABOUTTOSTART = 2,
        FORMCONVORUNNING = 3,
        FORMCONVERSATIONCOMPLETED = 4,
        REQUIRESPARTNERCALLFORNEXTRESPONSE = 5,
        CONVERSATION_WITH_ADMIN
    }
}

