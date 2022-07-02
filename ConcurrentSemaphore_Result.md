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
|           NoSynchronize |    41.31 ms |  0.825 ms |  1.740 ms |          - |         - |     - |     43 KB |
|             Interlocked |   102.00 ms |  2.002 ms |  2.225 ms |          - |         - |     - |     48 KB |
| SemaphoreSlim8WaitAsync |   101.10 ms |  0.641 ms |  0.568 ms |          - |         - |     - |    157 KB |
|  SemaphoreSlimWaitAsync | 1,040.63 ms | 16.013 ms | 14.195 ms | 11000.0000 | 1000.0000 |     - | 89,811 KB |
```
# Discussion
Max concurrency is capped at 10 threads. SemaphoeSlim8WaitAsync uses a Semaphoore(8,8) to allow up to 8 threads to concurrently add the sum via InterlockedAdd. SemaphoreSlimWaitAsync uses the exclusive Semaphore(1,1). Increasing max concurrency of the Semaphore to 8 produced **9 times faster** results. 