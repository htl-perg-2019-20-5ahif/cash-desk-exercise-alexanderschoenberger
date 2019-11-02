
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CashDesk
{
    public class Membership : IMembership
    {
        public int MembershipID { get; set; }

        [Required]
        public Member Member { get; set; }

        [Required]
        public DateTime Begin { get; set; }

        [Required]
        public DateTime End { get; set; }

        public List<Deposit> Deposits { get; set; }

        IMember IMembership.Member => Member;
    }
}
