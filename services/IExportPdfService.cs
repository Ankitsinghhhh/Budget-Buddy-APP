using Budget_Buddy_App.Models;
using System.Collections.Generic;

namespace Budget_Buddy_App.Services
{
    public interface IExportPdfService
    {
        byte[] GeneratePdf(List<Transaction> transactions);
    }
}
