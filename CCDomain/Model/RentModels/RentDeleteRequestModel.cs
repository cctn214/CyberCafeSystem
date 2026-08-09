using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.RentModels
{
    public class RentDeleteRequestModel
    {
        public int RentId { get; set; }
    }

    public class RentDeleteResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
