using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UserModels
{
    public class UserDetailRequestModel
    {
        public int UserId { get; set; }
    }

    public class UserDetailResponseModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
