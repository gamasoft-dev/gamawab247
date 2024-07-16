using System;

namespace Application.DTOs.CreateDialogDtos;

/// <summary>
/// This dto is used to create a business message for and save to the database.
/// Where T is the specific type of the message.(List, Button, TextMessage)
/// </summary>
/// <typeparam name="T"></typeparam>
public class CreateBusinessMessageDto<T> 
{
    public Guid BusinessId { get; set; }

    public int Position { get; set; }
    public string Name { get; set; }
    // This is the name of the message type (list, Reply Button, Text) etc.
    // public string MessageType { get; set; }
    public string RecipientType { get; set; }
    
    /// <summary>
    /// Use this to denote if a follow up message needs to be sent to the user immediately this is sent.
    /// </summary>
    public bool HasFollowUpMessage { get; set; }

    /// <summary>
    /// This is the parent of the message this message follow. This is optional.
    /// </summary>
    public Guid? FollowParentMessageId { get; set; }

    /// <summary>
    /// This means that the content of this business message would be retrieved from an
    /// external integration (local funciton or external api)
    /// When this value is set it would be likely that the InteractiveMessageId will be empty
    /// </summary>
    public Guid? ExternalContentRetrievalId { get; set; }

    /// <summary>
    /// This means that the conversation is to be taken over by an admin at this point, and automated response is sent to the user to notify them about this action
    /// </summary>
    public bool IsAdminResponseTriggered { get; set; }

    /// <summary>
    /// This is the object of the specific type of message that needs to be created
    /// </summary>
    public T? MessageTypeObject { get; set; }
}

/// <summary>
/// Response of a saved business message on retrieval
/// </summary>
/// <typeparam name="T"></typeparam>
public class BusinessMessageDto<T>
{
    public Guid Id { get; set; }
    public Guid BusinessId { get; set; }
    public Guid? BusinessConversationId { get; set; }

    public int Position { get; set; }
    public string Name { get; set; }
    // This is the name of the message type (list, Reply Button, Text) etc.
    public string MessageType { get; set; }
    public string RecipientType { get; set; }
    
    /// <summary>
    /// This is the object of the specific type of message that needs to be created
    /// </summary>
    public T MessageTypeObject { get; set; }
    public bool HasFollowUpMessage { get; set; }
    public Guid? FollowParentMessageId { get; set; }

    public string ContentRetrievalProcessorKey { get; set; }
    public bool ShouldRetrieveContentAtRuntime { get; set; }

    public bool ShouldTriggerFormProcessing { get; set; }
    public Guid? BusinessFormId { get; set; }
}

public class UpdateBusinessMessageDto<T> : CreateBusinessMessageDto<T> 
{
    public Guid Id { get; set; }
}