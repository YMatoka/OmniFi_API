using OmniFi_API.Models.Banks;
using System.Diagnostics.CodeAnalysis;

namespace OmniFi_API.Comparers
{
    public class BankSubAccountComparer : IEqualityComparer<BankSubAccount>
    {
        public bool Equals(BankSubAccount? x, BankSubAccount? y)
        {
            if(x == null || y == null) 
                return false;

            return x.BankSubAccountID == y.BankSubAccountID;
        }

        public int GetHashCode([DisallowNull] BankSubAccount obj)
        {
            //Allow arithmetic overflow
            unchecked
            {
                int hash = 17;
                return hash * 23 + obj.BankSubAccountID.GetHashCode();
            }

        }
    }
}
