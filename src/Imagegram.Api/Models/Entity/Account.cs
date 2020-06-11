using System;
using Dapper.Contrib.Extensions;

namespace Imagegram.Api.Models.Entity
{
    [Table("Accounts")]
    public class Account
    {
        [ExplicitKey]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}