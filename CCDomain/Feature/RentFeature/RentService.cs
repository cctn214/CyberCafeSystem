using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCDatabase.Models;
using CCDomain.Model.RentModels;

namespace CCDomain.Feature.RentFeature
{
    public class RentService
    {
        private readonly AppDbContext _context;
        public RentService(AppDbContext context)
        {
            _context = context;
        }

        public RentListResponseModel GetAllRents()
        {
            try
            {
                var list = _context.Rents.ToList();
                return new RentListResponseModel
                {
                    Rents = list.Select(r => new RentModel
                    {
                        RentId = r.RentId,
                        UserId = r.UserId,
                        UnitId = r.UnitId,
                        StartTime = r.StartTime,
                        EndTime = r.EndTime,
                        Duration = r.Duration,
                        TotalCost = r.TotalCost
                    }).ToList(),
                    IsSuccess = true,
                    Message = "Rents retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new RentListResponseModel
                {
                    IsSuccess = false,
                    Message = ex.ToString(),
                };
            }
        }

        public RentDetailResponseModel GetRentById(RentDetailRequestModel request)
        {
            try
            {
                var rent = _context.Rents.FirstOrDefault(r => r.RentId == request.RentId);
                if (rent == null)
                {
                    return new RentDetailResponseModel
                    {
                        IsSuccess = false,
                        Message = "Rent not found"
                    };
                }

                return new RentDetailResponseModel
                {
                    RentId = rent.RentId,
                    UserId = rent.UserId,
                    UnitId = rent.UnitId,
                    StartTime = rent.StartTime,
                    EndTime = rent.EndTime,
                    Duration = rent.Duration,
                    TotalCost = rent.TotalCost,
                    IsSuccess = true,
                    Message = "Rent found"
                };
            }
            catch (Exception ex)
            {
                return new RentDetailResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public RentCreateResponseModel CreateRent(RentCreateRequestModel request)
        {
            try
            {
                var duration = request.EndTime.Hour - request.StartTime.Hour;
                var rent = new Rent
                {
                    UserId = request.UserId,
                    UnitId = request.UnitId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Duration = duration,
                 
                };
                var usedUnitInRent = _context.Units.FirstOrDefault(u => u.UnitId == request.UnitId);
                rent.TotalCost = usedUnitInRent != null ? usedUnitInRent.Rate * duration : 0;
                _context.Rents.Add(rent);
                _context.SaveChanges();
                return new RentCreateResponseModel
                {
                    RentId = rent.RentId,
                    UserId = rent.UserId,
                    UnitId = rent.UnitId,
                    StartTime = rent.StartTime,
                    EndTime = rent.EndTime,
                    Duration = rent.Duration,
                    TotalCost = rent.TotalCost,
                    IsSuccess = true,
                    Message = "Rent created successfully"
                };
            }
            catch (Exception ex)
            {
                return new RentCreateResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public RentPatchResponseModel UpdateRent(RentPatchRequestModel request, int rentId)
        {
            try
            {
                var rent = _context.Rents.FirstOrDefault(r => r.RentId == rentId);
                if (rent == null)
                {
                    return new RentPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Rent not found"
                    };
                }
                if (request.UserId.HasValue)
                {
                    rent.UserId = request.UserId.Value;
                }
                if (request.UnitId.HasValue)
                {
                    rent.UnitId = request.UnitId.Value;
                }
                if (request.StartTime.HasValue)
                {
                    rent.StartTime = request.StartTime.Value;
                }
                if (request.EndTime.HasValue)
                {
                    rent.EndTime = request.EndTime.Value;
                }
                if (request.Duration.HasValue)
                {
                    rent.Duration = request.Duration.Value;
                }
                if (request.TotalCost.HasValue)
                {
                    rent.TotalCost = request.TotalCost.Value;
                }
                _context.SaveChanges();
                return new RentPatchResponseModel
                {
                    UserId = rent.UserId,
                    UnitId = rent.UnitId,
                    StartTime = rent.StartTime,
                    EndTime = rent.EndTime,
                    Duration = rent.Duration,
                    TotalCost = rent.TotalCost,
                    IsSuccess = true,
                    Message = "Rent updated successfully"
                };
            }
            catch (Exception ex)
            {
                return new RentPatchResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public RentDeleteResponseModel DeleteRent(RentDeleteRequestModel request)
        {
            try
            {
                var rent = _context.Rents.FirstOrDefault(x => x.RentId == request.RentId);
                if (rent == null)
                {
                    return new RentDeleteResponseModel
                    {
                        IsSuccess = false,
                        Message = "Rent not found"
                    };
                }
                _context.Rents.Remove(rent);
                _context.SaveChanges();
                return new RentDeleteResponseModel
                {
                    IsSuccess = true,
                    Message = "Rent deleted successfully"
                };
            }
            catch (Exception)
            {
                return new RentDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the rent"
                };
            }
        }
    }
}
