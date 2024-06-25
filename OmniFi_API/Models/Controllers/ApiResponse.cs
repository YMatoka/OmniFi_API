using System.Net;

namespace OmniFi_API.Models.Controllers
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode {get; set;}
        public bool IsSuccess { get; set; } = true;
        public List<string>? ErrorMessages { get; set; }
        public object? Result { get; set; }

        public void AddErrorMessage(string message)
        {
            if(ErrorMessages is null) 
                ErrorMessages = new List<string>(); 

            ErrorMessages.Add(message);
        }
        public void AddErrorMessage(IEnumerable<string> messages)
        {
            if (ErrorMessages is null)
                ErrorMessages = new List<string>();

            ErrorMessages.AddRange(messages);
        }
    }
}
