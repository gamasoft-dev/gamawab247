using BillProcessorAPI.Entities;

namespace BillProcessorAPI.Repositories.Interfaces
{
    public interface IInvoiceRepository:IRepository<Invoice>
    {
        Task<Invoice> GetBillInvoiceWithReceipt(string Id);
    }
}
