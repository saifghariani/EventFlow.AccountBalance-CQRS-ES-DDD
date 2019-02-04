using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AccountBalance.Api.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DepositType
    {
        Cash,
        Check
    }
}
