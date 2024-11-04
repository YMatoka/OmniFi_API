namespace OmniFi_API.Utilities
{
    public static class ErrorMessages
    {
        public const string Error500Message = "An unexpected error occurred. Please try again later.";

        public const string VariableTag = "{VariableTag}";

        public const string ErrorUserNotFoundMessage = $"The usernameOrEmail or email '{VariableTag}' does not exists";

        public const string ErrorUnauthorizedAccessMessage = $"The credentials of your '{VariableTag}' account are incorrect, " +
            $"the system is unable to retrieve its data";

        public const string ErrorGetMethodMessage = $"Error Performing GET in '{VariableTag}' method";
        public const string ErrorPostMethodMessage = $"Error Performing POST in '{VariableTag}' method";
        public const string ErrorPutMethodMessage = $"Error Performing PUT in '{VariableTag}' method";
        public const string ErrorDeleteMethodMessage = $"Error Performing DELETE in '{VariableTag}' method";
        public const string ErrorCreateMethodMessage = $"Error Performing CREATE in '{VariableTag}' method";

    }
}
