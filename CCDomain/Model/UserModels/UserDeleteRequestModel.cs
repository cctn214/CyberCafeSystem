using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UserModels
{
    public class UserDeleteRequestModel
    {
        public int UserId { get; set; }
    }

    public class UserDeleteResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
