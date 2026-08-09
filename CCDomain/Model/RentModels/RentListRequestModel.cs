using System;
using System.Collections.Generic;
using System.Text;

namespace CCDomain.Model.RentModels
{
    public class RentListRequestModel
    {
    }

    public class RentListResponseModel
    {
        public List<RentModel> Rents { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class RentModel
    {
        public int RentId { get; set; }
        public int UserId { get; set; }
        public int UnitId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; }
        public decimal? TotalCost { get; set; }
    }
}
