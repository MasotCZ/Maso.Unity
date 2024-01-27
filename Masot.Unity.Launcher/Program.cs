

using BenchmarkDotNet.Running;

namespace Masot.Unity.Launcher
{
    class Program
    {
	// for testing purposes only
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }
}