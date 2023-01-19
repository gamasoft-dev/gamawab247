namespace Domain.Common
{
    public static class ResponseMessages
    {
        public const string DuplicateEmail = "Email already exist";
        public const string RetrievalSuccessResponse = "Data retrieved successfully";
        public const string CreationSuccessResponse = "Data created successfully";
        public const string LoginSuccessResponse = "Login successful"; 
        public const string WrongEmailOrPassword = "Wrong email or password provided";
        public const string UserNotFound = "User not found";
        public const string InvalidToken = "Invalid token";
        public const string ExpiredToken = "Expired token";
        public const string MissingClaim = "MissingClaim:";
        public const string PasswordCannotBeEmpty = "Password cannot be empty";
        public const string ParameterCannotBeNull = "Parameter cannot be null";
        public const string PayLoadCannotBeNull = "Payload cannot be null";
        public const string Successful = "Successful";
        public const string Failed = "Failed";
        public const string IncorrectId = "Incorrect Business Id";
        public const string BusinessSettingsNotFound = "Business settings does not exist";
        public const string BusinessNotFound = "Business not found";
        public const string BusinessMessageNotFound = "Business message not found";
        public const string ListMessageNotFound = "List message not found";
        public const string TextMessageNotFound = "Text message not found";
        public const string UpdateResponse = "Update success";
        public const string EmailAlreadyComfirmed = "Email Already Comfirmed";
        public const string WhatsappUserNotFound = "Whatsapp User Not Found";
        public const string UserFormDataNotFound = "User form data Not Found";
        public const string DialogSessionNotFound = "Dialog sessiion Not Found";
        public const string UserFormRequestNotFound = "User form requet response Not Found";

        public static string Duplicity { get; set; }
        public static string LogNotFound { get; set; }
    }
}