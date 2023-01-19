namespace Domain.Enums;

public enum EResponseProcessingStatus
{
    Pending,
    Processing,
    Sent,
    Failed,
    Recieved,
    SessionExpired,
    InValidSessionState,
    ProcessCompleted
}