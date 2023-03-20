using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.RequestAndComplaintDtos;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IRequestAndComplaintConfigService:IAutoDependencyService
    {
        Task<SuccessResponse<RequestAndComplaintConfigDto>> CreateRequestAndComplaintConfig(CreateRequestAndComplaintConfigDto model);
        Task<SuccessResponse<RequestAndComplaintConfigDto>> UpdateRequestAndComplaintConfig(Guid id, UpdateRequestAndComplaintConfigDto model);
        Task<SuccessResponse<bool>> DeleteRequestAndComplaintConfig(Guid id);
        Task<SuccessResponse<RequestAndComplaintConfigDto>> GetRequestAndComplaintConfig(Guid id);        Task<PagedResponse<IEnumerable<RequestAndComplaintConfigDto>>> GetAllRequestAndComplaintConfig(ResourceParameter parameter,
            string endPointName, IUrlHelper url);
        Task<PagedResponse<IEnumerable<RequestAndComplaintConfigDto>>> GetAllRequestAndComplaintConfigByBusinessId(Guid businessId, ResourceParameter parameter,
            string endPointName, IUrlHelper url);
    }
}
