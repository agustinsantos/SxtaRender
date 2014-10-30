
namespace Sxta.Core
{
    public interface IDeepCloneable
    {
        object DeepClone();
    }

    public interface IDeepCloneable<T> : IDeepCloneable
    {
        new T DeepClone();
    }
}
