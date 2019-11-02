using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {
        private MemberContext memberContext;

        /// <inheritdoc />
        public Task InitializeDatabaseAsync()
        {
            if (memberContext != null)
            {
                throw new InvalidOperationException("Already initialized");
            }

            memberContext = new MemberContext();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<int> AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            if (memberContext == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || birthday == null)
            {
                throw new ArgumentException("At least one of the parameters contains an invalid value");
            }
            if (await memberContext.Members.AnyAsync(m => m.LastName == lastName))
            {
                throw new DuplicateNameException($"Member with the same last name:{lastName} already exists");
            }
            var newMember = new Member
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday
            };
            memberContext.Members.Add(newMember);

            await memberContext.SaveChangesAsync();
            return newMember.MemberNumber;
        }

        /// <inheritdoc />
        public async Task DeleteMemberAsync(int memberNumber)
        {
            if (memberContext == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            Member memberToRemove;
            try
            {
                memberToRemove = await memberContext.Members.FirstAsync(m => m.MemberNumber == memberNumber);
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException();
            }
            memberContext.Members.Remove(memberToRemove);
            await memberContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IMembership> JoinMemberAsync(int memberNumber)
        {
            if (memberContext == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            if (await memberContext.Memberships.AnyAsync(m => m.Member.MemberNumber == memberNumber
               && m.End == DateTime.MaxValue))
            {
                throw new AlreadyMemberException();
            }
            var membership = new Membership
            {
                Member = await memberContext.Members.FirstAsync(m => m.MemberNumber == memberNumber),
                Begin = DateTime.Now,
                End = DateTime.MaxValue
            };
            memberContext.Memberships.Add(membership);
            await memberContext.SaveChangesAsync();
            return membership;
        }



        /// <inheritdoc />
        public async Task<IMembership> CancelMembershipAsync(int memberNumber)
        {
            if (memberContext == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            Membership membership;
            try
            {
                membership = await memberContext.Memberships.FirstAsync(m => m.Member.MemberNumber == memberNumber
                    && m.End == DateTime.MaxValue);
            }
            catch (InvalidOperationException)
            {
                throw new NoMemberException();
            }
            membership.End = DateTime.Now;
            await memberContext.SaveChangesAsync();
            return membership;
        }

        /// <inheritdoc />
        public async Task DepositAsync(int memberNumber, decimal amount)
        {
            if (memberContext == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            Member member;
            try
            {
                member = await memberContext.Members.FirstAsync(m => m.MemberNumber == memberNumber);
            }
            catch (InvalidOperationException)
            {
                throw new NoMemberException();
            }
            Membership membership;
            try
            {
                membership = await memberContext.Memberships.FirstAsync(m => m.Member.MemberNumber == memberNumber
                    && m.End >= DateTime.Now);
            }
            catch (InvalidOperationException)
            {
                throw new NoMemberException();
            }
            var deposit = new Deposit { Membership = membership, Amount = amount };
            memberContext.Deposits.Add(deposit);
            await memberContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IDepositStatistics>> GetDepositStatisticsAsync()
        {
            if (memberContext == null)
            {
                throw new InvalidOperationException("Not initialized");
            }
            var depositStatistics = new List<DepositStatistics>();
            foreach (var member in memberContext.Members)
            {
                decimal sum = 0;
                if (member.Memberships != null)
                {
                    foreach (var membership in member.Memberships)
                    {
                        foreach (var test in membership.Deposits)
                        {
                            sum += test.Amount;
                        }
                    }

                    depositStatistics.Add(new DepositStatistics
                    {
                        Member = member,
                        TotalAmount = sum,
                        Year = member.Memberships.First().Begin.Year
                    });
                }
            }
            return depositStatistics;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (memberContext != null)
            {
                memberContext.Dispose();
                memberContext = null;
            }
        }
    }
}