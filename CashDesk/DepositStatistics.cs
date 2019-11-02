using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk
{
    public class DepositStatistics : IDepositStatistics
    {
        public IMember Member { get; set; }

        public int Year { get; set; }

        public decimal TotalAmount { get; set; }
   //     IMember IDepositStatistics.Member => Member;
    }
}
