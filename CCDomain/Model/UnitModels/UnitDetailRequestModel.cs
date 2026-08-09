using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UnitModels
{
    public class UnitDetailRequestModel
    {
        public int UnitId { get; set; }
    }

    public class UnitDetailResponseModel
    {
        public int UnitId { get; set; }
        public string Type { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
