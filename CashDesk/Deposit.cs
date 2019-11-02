using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CashDesk
{
    public class Deposit : IDeposit
    {
        public int DepositID { get; set; }

        [Required]
        public Membership Membership { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public decimal Amount { get; set; }

        IMembership IDeposit.Membership => Membership;
    }
}
