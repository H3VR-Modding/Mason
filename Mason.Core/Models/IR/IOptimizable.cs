namespace Mason.Core.IR
{
	internal interface IOptimizable<out T> : INopOptimizable<T> where T : IOptimizable<T>
	{
		new T Optimize();
	}
}
