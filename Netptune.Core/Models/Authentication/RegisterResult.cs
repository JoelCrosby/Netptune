﻿using Netptune.Core.Authentication.Models;

namespace Netptune.Core.Models.Authentication
{
    public class RegisterResult
    {
        public bool IsSuccess { get; private set; }

        public AuthenticationTicket Ticket { get; }

        public string Message { get; }

        private RegisterResult(bool success, AuthenticationTicket ticket, string message = null)
        {
            IsSuccess = success;
            Ticket = ticket;
            Message = message;
        }

        public static RegisterResult Success(AuthenticationTicket ticket)
        {
            return new RegisterResult(true, ticket);
        }

        public static RegisterResult Failed(string message = null)
        {
            return new RegisterResult(false, null, message);
        }
    }
}