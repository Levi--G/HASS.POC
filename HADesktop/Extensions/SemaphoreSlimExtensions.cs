using System;
using System.Collections.Generic;
using System.Text;

namespace HADesktop.Extensions
{
    internal static class SemaphoreSlimExtensions
    {
        public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            return new SemaphoreDisposable(semaphore);
        }
        public static IDisposable Lock(this SemaphoreSlim semaphore)
        {
            semaphore.Wait();
            return new SemaphoreDisposable(semaphore);
        }

        private class SemaphoreDisposable(SemaphoreSlim semaphore) : IDisposable
        {
            public void Dispose()
            {
                semaphore.Release();
            }
        }
    }
}
