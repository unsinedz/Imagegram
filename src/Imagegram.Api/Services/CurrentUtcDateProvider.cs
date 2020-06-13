using System;

namespace Imagegram.Api.Services
{
    public class CurrentUtcDateProvider : ICurrentUtcDateProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}