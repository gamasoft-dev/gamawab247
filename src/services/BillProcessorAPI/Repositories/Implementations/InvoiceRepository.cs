using BillProcessorAPI.Data;
using BillProcessorAPI.Entities;
using BillProcessorAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BillProcessorAPI.Repositories.Implementations
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(BillProcessorDbContext context)
          : base(context)
        {
           
        }

        public async Task<Invoice> GetBillInvoiceWithReceipt(string Id)
        {
            return await _context.Invoices.Include(w=>w.Receipts).FirstOrDefaultAsync(x => x.TransactionReference == Id);
        }

        //public async Task<Invoice> GetBillTransactionsWithReceipt(string Id)
        //{
        //        

        //}
    }
}
