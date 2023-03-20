using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.AutofacDI;
using Application.DTOs.PartnerContentDtos;
using Application.DTOs.RequestAndComplaintDtos;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IRequestAndComplaintService : IAutoDependencyService
    {
        Task<SuccessResponse<RequestAndComplaintDto>> CreateRequestAndComplaint(CreateRequestAndComplaintDto model);
        Task<SuccessResponse<RequestAndComplaintDto>> UpdateRequestAndComplaint(Guid id,UpdateRequestAndComplaintDto model);
        Task<SuccessResponse<bool>> DeleteRequestAndComplaint(Guid id);
        Task<SuccessResponse<RequestAndComplaintDto>> GetRequestAndComplaint(Guid id);
        Task<SuccessResponse<RequestAndComplaintDto>> GetRequestAndComplaintByTicketId(string ticketId);
        Task<PagedResponse<IEnumerable<RequestAndComplaintDto>>> GetAllRequestAndComplaint(ResourceParameter parameter,
            string endPointName, IUrlHelper url);
        Task<PagedResponse<IEnumerable<RequestAndComplaintDto>>> GetAllRequestAndComplaintByBusinessId(Guid businessId, ResourceParameter parameter,
            string endPointName, IUrlHelper url);

    }
}
