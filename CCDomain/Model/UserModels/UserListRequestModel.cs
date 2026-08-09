using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UserModels
{
    public class UserListRequestModel
    {
    }

    public class UserListResponseModel
    {
        public List<UserModel> Users { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

    }

    public class UserModel {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }
}
