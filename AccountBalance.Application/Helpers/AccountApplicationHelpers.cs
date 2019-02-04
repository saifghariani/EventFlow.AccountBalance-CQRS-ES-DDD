using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AccountBalance.Application.Helpers
{
    public static class AccountApplicationHelpers
    {
        public static Assembly Assembly { get; } = typeof(AccountApplicationHelpers).Assembly;
    }
}
