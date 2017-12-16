using System.Collections.Generic;

namespace KerbalEngineer
{
    /// <summary>
    ///     Pool of objects, each object gets reset (by delegate) before being reused.
    ///     THREAD SAFE.
    /// </summary>
    public class Pool<T>
    {
        private readonly Stack<T> values = new Stack<T>();

        private readonly CreateDelegate create;
        private readonly ResetDelegate reset;

        public delegate T CreateDelegate();
        public delegate void ResetDelegate(T value);
        
        /// <summary>
        ///     Creates an empty pool with the specified object creation and reset delegates.
        ///     Reset function can be null, so no action is performed on an object.
        /// </summary>
        public Pool(CreateDelegate create, ResetDelegate reset = null)
        {
            this.create = create;
            this.reset = reset ?? (x => {});
        }

        /// <summary>
        ///     Borrows an object from the pool.
        /// </summary>
        public T Borrow()
        {
            lock (values)
            {
                if (values.Count > 0) 
                    return values.Pop();
            }
            T value = create();
            reset(value);
            return value;
        }
        
        /// <summary>
        ///     Release an object, reset it and return it to the pool.
        /// </summary>
        public void Release(T value)
        {
            reset(value);
            lock (values)
            {
                values.Push(value);
            }
        }
        
        /// <summary>
        ///     Number of objects currently available for use.
        /// </summary>
        public int Count
        {
            get
            {
                lock (values)
                {
                    return values.Count;
                }
            }
        }
        
    }
}
