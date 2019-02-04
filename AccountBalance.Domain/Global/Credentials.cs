using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI.SystemData;

namespace AccountBalance.Domain.Global
{
    public static class Credentials
    {
        public static UserCredentials AdminCredentials = new UserCredentials("admin", "changeit");
    }
}
