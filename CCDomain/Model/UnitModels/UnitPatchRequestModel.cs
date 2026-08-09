using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.UnitModels
{
    public class UnitPatchRequestModel
    {
        public string? Type { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UnitPatchResponseModel
    {
        public string Type { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
