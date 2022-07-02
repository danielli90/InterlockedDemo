using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentSemaphore
{
    public class ConcurrentSemaphoreDemo
    {
        private static ParallelOptions _options =  new ParallelOptions { MaxDegreeOfParallelism = 10 };

        static void Main(string[] args)
        {
            var src = Enumerable.Range(1, 100_000);

            Measure(NoSynchronize, src, "WARM_UP");
            Measure(NoSynchronize, src, nameof(NoSynchronize));
            Measure(InterlockedAdd, src, nameof(InterlockedAdd));
            Measure(WithSemaphoreSlim8WaitAsync, src, nameof(WithSemaphoreSlim8WaitAsync));
            Measure(WithSemaphoreSlimWaitAsync, src, nameof(WithSemaphoreSlimWaitAsync));
            // Measure(WithSemaphoreSlimWait, src, nameof(WithSemaphoreSlimWait));
        }

        public static void Measure(Func<IEnumerable<int>, long> func, IEnumerable<int> src, string caseName)
        {
            var sw = new Stopwatch();
            sw.Start();
            var sum = func(src);
            sw.Stop();
            var ts = sw.ElapsedMilliseconds;
            Console.WriteLine($"Result for {caseName}: {sum}.Runtime: {ts}ms");
        }

        public static long InterlockedAdd(IEnumerable<int> src)
        {
            long sum = 0;
            long sum2 = 0;
            long sum3 = 0;

            // Console.WriteLine("ConcurrentSemaphore Start");
            Parallel.ForEach(
                src, 
                _options,
                n => 
                {
                    Interlocked.Add(ref sum, n);
                    Interlocked.Add(ref sum2, n);
                    Interlocked.Add(ref sum3, n);
                    // Console.WriteLine(@"[ConcurrentSemaphore] value of n = {0}, thread = {1}", n, Thread.CurrentThread.ManagedThreadId);
                }
            );
            // Console.WriteLine("ConcurrentSemaphore End");
            return sum;
        }

        private static long WithSemaphoreSlimWaitAsyncInternal(IEnumerable<int> src, int concurrency = 1)
        {
            long sum = 0;
            long sum2 = 0;
            long sum3 = 0;

            var semaphore = new SemaphoreSlim(concurrency, concurrency);
            // Console.WriteLine("SemaphoreSlim Start");
            Parallel.ForEach(
                src, 
                _options,
                n => {
                    semaphore.WaitAsync();
                    sum += n;
                    sum2 += n;
                    sum3 += n;
                    semaphore.Release();
                    // Console.WriteLine(@"[Semaphore] value of n = {0}, thread = {1}", n, Thread.CurrentThread.ManagedThreadId);
                }
            );
            semaphore.Dispose();
            // Console.WriteLine("SemaphoreSlim End");
            return sum;
        }

        public static long WithSemaphoreSlimWaitAsync(IEnumerable<int> src)
        {
            return WithSemaphoreSlimWaitAsyncInternal(src, 1);
        }

        public static long WithSemaphoreSlim8WaitAsync(IEnumerable<int> src)
        {
            return WithSemaphoreSlimWaitAsyncInternal(src, 8);
        }

        public static long WithSemaphoreSlimWait(IEnumerable<int> src)
        {
            long sum = 0;
            long sum2 = 0;
            long sum3 = 0;

            var semaphore = new SemaphoreSlim(1, 1);
            // Console.WriteLine("SemaphoreSlim Start");
            Parallel.ForEach(
                src, 
                _options,
                n => {
                    semaphore.Wait();
                    sum += n;
                    sum2 += n;
                    sum3 += n;
                    semaphore.Release();
                    // Console.WriteLine(@"[Semaphore] value of n = {0}, thread = {1}", n, Thread.CurrentThread.ManagedThreadId);
                }
            );
            semaphore.Dispose();
            // Console.WriteLine("SemaphoreSlim End");
            return sum;
        }

        public static long NoSynchronize(IEnumerable<int> src)
        {
            long sum = 0;
            long sum2 = 0;
            long sum3 = 0;
            // Console.WriteLine("NoSync Start");
            Parallel.ForEach(
                src,
                _options,
                n =>
                {
                    sum += n;
                    sum2 += n;
                    sum3 += n;
                    // Console.WriteLine(@"[NoSync] value of n = {0}, thread = {1}", n, Thread.CurrentThread.ManagedThreadId);
                }
            );
            // Console.WriteLine("NoSync End");
            return sum;
        }
    }
}
