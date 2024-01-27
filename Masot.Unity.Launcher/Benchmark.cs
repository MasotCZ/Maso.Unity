
using BenchmarkDotNet.Attributes;
using Masot.Framework.DataStructures.Factories;
using Masot.Unity.DataStructures.DataPools;

namespace Masot.Unity.Launcher
{
    public struct Options : IRefPoolOptions<A>
    {
        public Options(IDataPoolFactory<A> dataFactory, int initialSize)
        {
            DataFactory = dataFactory;
            InitialSize = initialSize;
        }

        public IDataPoolFactory<A> DataFactory { get; init; }
        public int InitialSize { get; init; }
    }

    public struct AFactory : IDataPoolFactory<A>
    {
        public A Create()
        {
            var ret = new A();
            ret.IsValid = true;
            return ret;
        }

        public void Decommission(A toDecom)
        {
            toDecom.IsValid = false;
        }
    }

    public class A
    {
        public bool IsValid { get; set; }
    }

    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    class Benchmark
    {
        private IRefDataPool<A> naivePool;

        public void Init(int count)
        {
            naivePool = new RefDataPoolNaive<A>(new Options { DataFactory = new AFactory(), InitialSize = count });
        }

        [Benchmark]
        public void CreateNaive()
        {
            naivePool.GetOrCreate();
        }

        public void ManipulateNaive(int count)
        {
            for (int i = 0; i < count; i++)
            {
                int rnd = new Random(count).Next();
                naivePool.Decommission(naivePool.GetOrCreate());
            }
        }
    }
}