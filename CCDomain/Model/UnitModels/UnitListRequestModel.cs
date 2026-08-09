using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UnitModels
{
    public class UnitListRequestModel
    {
    }

    public class UnitListResponseModel
    {
        public List<UnitModel> Units { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class UnitModel
    {
        public int UnitId { get; set; }
        public string Type { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
    }
}
