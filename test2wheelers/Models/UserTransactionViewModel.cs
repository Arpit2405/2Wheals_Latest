using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2whealers.Models
{
    public class UserTransactionViewModel
    { 
        public TransactionViewModel Transaction { get; set; } = new TransactionViewModel();
        public PurchaseViewModel Purchase { get; set; } = new PurchaseViewModel();
    }
}