# Setup
Azure VM Size: Standard DS5 v2 (16 vcpus, 56 GiB memory)
Operating System: Standard DS5 v2 (16 vcpus, 56 GiB memory)

# ConconrrentSemaphore Result
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.17763.3046 (1809/October2018Update/Redstone5), VM=Hyper-V
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 16 logical and 16 physical cores
  [Host]     : .NET Framework 4.7.2 (4.7.3946.0), X64 RyuJIT
  DefaultJob : .NET Framework 4.7.2 (4.7.3946.0), X64 RyuJIT


```
|                  Method |        Mean |     Error |    StdDev |      Gen 0 |     Gen 1 | Gen 2 | Allocated |
|------------------------ |------------:|----------:|----------:|-----------:|----------:|------:|----------:|
|           NoSynchronize |    41.62 ms |  0.828 ms |  1.936 ms |          - |         - |     - |     44 KB |
|             Interlocked |   111.50 ms |  2.192 ms |  3.000 ms |          - |         - |     - |     48 KB |
| SemaphoreSlim8WaitAsync |   101.32 ms |  0.547 ms |  0.485 ms |          - |         - |     - |     88 KB |
|  SemaphoreSlimWaitAsync | 1,048.90 ms | 20.630 ms | 33.896 ms | 11000.0000 | 1000.0000 |     - | 90,131 KB |

# Discussion
Max concurrency is capped at 10 threads. SemaphoeSlim8WaitAsync uses a Semaphoore(8,8) to allow up to 8 threads to concurrently add the sum via InterlockedAdd. SemaphoreSlimWaitAsync uses the exclusive Semaphore(1,1). Increasing max concurrency of the Semaphore to 8 produced 9 times faster results. 
