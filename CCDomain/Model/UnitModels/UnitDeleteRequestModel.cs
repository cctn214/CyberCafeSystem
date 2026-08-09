using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UnitModels
{
    public class UnitDeleteRequestModel
    {
        public int UnitId { get; set; }
    }

    public class UnitDeleteResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
