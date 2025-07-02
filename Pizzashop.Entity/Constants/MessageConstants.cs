namespace Pizzashop.Entity.Constants;

public static class MessageConstants
{
    public const string EditMessage = "Updated Successfully!";
    public const string CreateMessage = "Created Successfully!";
    public const string DeleteMessage = "Deleted Successfully!";
    public const string GetMessage = "Fetched Successfully!";
    public const string NotFoundMessage = "Not Found!";
    public const string ExportSuccessMessage = "Exported Successfully!";
    public const string AlreadyExistMessage = "Already Exists!";
    public const string SaveMessage = "Saved Successfully!";
    public const string CompleteMessage = "Completed Successfully!";
    public const string CancelMessage = "Cancelled Successfully!";

    public const string TableNotDeleteMessage = "Occupied table can't be deleted!";
    public const string InvalidCredentialsMessage = "Invalid Password!";
    public const string NewPasswordMustDifferentMessage = "New Password must be different from Current Password!";
    public const string LoginSuccessMessage = "User Login Successfully!";
    public const string UserInactiveMessage = "Your Account is inactive!";
    public const string ResetPasswordLinkSentMessage = "Reset password link has been sent to your Email Address!";
    public const string InvalidTokenMessage = "Token is invalid or expired!";
    public const string PasswordResetMessage = "Password Reset Successfully!";
    public const string InvalidCurrentPasswordMessage = "Invalid Current Password!";

    public const string TableAssignMessage = "Table(s) Assigned Successfully!";
    public const string SectionNotDeleteMessage = "One or More Table is occupied in this section!";
    public const string WaitingTokenAlreadyExistMessage = "Waiting Token For Customer Already Exist!";
    public const string CustomerOrderInRunningMessage = "Customer Has Running Order!";
    public const string ExceedTableCapacityMessage = "Number of person must be less than table capacity!";

    public const string ItemAlreadyPreparedMessage = "Ordered Item Already Prepared!";
    public const string ItemNotPreparedMessage = "Ordered Item Not Prepared!";
    public const string FailedToCompleteOrderMessage = "All items must be served before completing the orders!";
    public const string OrderNotServedMessage = "Order not served yet!";
    public const string OrderServedMessage = "Order is Served!";
    public const string OrderNotCancelMessage = "The order item is ready, can not cancel the order!";

    public const string DefaultErrorMessage = "An error occurred while processing your request.";
}
