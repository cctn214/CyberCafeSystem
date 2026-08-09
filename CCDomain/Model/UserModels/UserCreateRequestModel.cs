using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UserModels
{
    public class UserCreateRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    public class UserCreateResponseModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
