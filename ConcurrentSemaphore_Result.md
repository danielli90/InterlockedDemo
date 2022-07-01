# Setup
Azure VM Size: Standard DS5 v2 (16 vcpus, 56 GiB memory)
Operating System: Standard DS5 v2 (16 vcpus, 56 GiB memory)

# ConconrrentSemaphore Result
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.17763.3046 (1809/October2018Update/Redstone5), VM=Hyper-V
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 16 logical and 16 physical cores
  [Host]     : .NET Framework 4.7.2 (4.7.3946.0), X64 RyuJIT
  DefaultJob : .NET Framework 4.7.2 (4.7.3946.0), X64 RyuJIT


```
|                  Method |      Mean |     Error |    StdDev |      Gen 0 |     Gen 1 | Gen 2 | Allocated |
|------------------------ |----------:|----------:|----------:|-----------:|----------:|------:|----------:|
|           NoSynchronize |  39.28 ms |  0.605 ms |  0.536 ms |          - |         - |     - |     49 KB |
|             Interlocked |  97.58 ms |  1.932 ms |  1.807 ms |          - |         - |     - |     52 KB |
| SemaphoreSlim8WaitAsync | 150.08 ms |  2.966 ms |  7.163 ms |          - |         - |     - |  3,658 KB |
|  SemaphoreSlimWaitAsync | 686.94 ms | 11.970 ms | 11.197 ms | 11000.0000 | 1000.0000 |     - | 91,123 KB |

# Discussion
Max concurrency is capped at 12 threads. SemaphoeSlim8WaitAsync uses a Semaphoore(8,8) to allow up to 8 threads to concurrently add the sum via InterlockedAdd. SemaphoreSlimWaitAsync uses the exclusive Semaphore(1,1). Increasing max concurrency of the Semaphore to 8 produced 3 times faster results. 
