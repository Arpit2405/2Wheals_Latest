namespace _2whealers.Models
{
    public class BranchDetailsViewModel
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; }

        public int SalesCount { get; set; }
        public int PreSalesCount { get; set; }

        public List<PurchaseViewModel> PreSales { get; set; } = new();
        public List<TransactionViewModel> Sales { get; set; } = new();
    }
}
