using System;

namespace Imagegram.Api.Services
{
    public interface ICurrentUtcDateProvider
    {
        DateTime UtcNow { get; }
    }
}