using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.OperationsHistory.Models
{
    public class ErrorResponse
    {
        public Dictionary<string, List<string>> ErrorMessages { get; } = new Dictionary<string, List<string>>();

        public static ErrorResponse Create(string message)
        {
            var response = new ErrorResponse();

            response.ErrorMessages.Add(string.Empty, new List<string> {message});

            return response;
        }

        public static ErrorResponse Create(string field, string message)
        {
            var response = new ErrorResponse();

            response.ErrorMessages.Add(field, new List<string> {message});

            return response;
        }
    }
}
