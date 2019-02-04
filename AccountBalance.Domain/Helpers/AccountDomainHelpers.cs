using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AccountBalance.Domain.Helpers
{
    public static class AccountDomainHelpers
    {
        public static Assembly Assembly { get; } = typeof(AccountDomainHelpers).Assembly;

    }
}
