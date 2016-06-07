using System;

namespace Sxta.OSG
{
    public static class CloneUtil<T> where T : BaseObject
    {
        public static T Clone(T t, CopyOp copyop = CopyOp.SHALLOW_COPY)
        {
            if (t != null)
            {
                T obj = t.Clone(copyop) as T;
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    Console.WriteLine("Warning: CloneUtil.Clone(T t, CopyOp copyop = CopyOp.SHALLOW_COPY) cloned object not of type T, returning null.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Warning: CloneUtil.Clone(T t, CopyOp copyop = CopyOp.SHALLOW_COPY) passed null object to clone, returning null.");
                return null;
            }
        }

        public static T Clone(T t, string name, CopyOp copyop = CopyOp.SHALLOW_COPY)
        {
            T newObject = Clone(t, copyop);
            if (newObject != null)
            {
                newObject.Name = name;
                return newObject;
            }
            else
            {
                Console.WriteLine("Warning: CloneUtil.Clone(T t, string name, CopyOp copyop = CopyOp.SHALLOW_COPY) passed null object to clone, returning null.");
                return null;
            }
        }

        public static T CloneType(T t)
        {
            if (t != null)
            {
                T obj = t.CloneType() as T;

                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    Console.WriteLine("Warning: CloneUtil.CloneType(T t) cloned object not of type T, returning null.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("Warning: CloneUtil.CloneType(T t) passed null object to clone, returning null.");
                return null;
            }
        }
    }
}