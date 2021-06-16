namespace Mason.Core.IR
{
	internal interface INopOptimizable<out T> where T : INopOptimizable<T>
	{
		T? Optimize();
	}
}
