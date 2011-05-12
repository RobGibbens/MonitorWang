using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace MonitorWang.Core
{
    public class InterceptorSelector<T> : IInterceptorSelector
    {
        protected string myMethodToIntercept;

        public InterceptorSelector(string methodToIntercept)
        {
            myMethodToIntercept = methodToIntercept;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            var active = new List<IInterceptor>(interceptors);

            if (string.CompareOrdinal(method.Name, myMethodToIntercept) != 0)
            {
                // play nicely with other filters, remove our publisher filters
                // if the property/method being intercepted is not "Consume"
                active.RemoveAll(i => i is T);
                return active.ToArray();
            }

            // ok, something has called the Consume method
            return active.ToArray();
        }
    }
}