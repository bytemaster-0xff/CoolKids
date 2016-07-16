using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Common.IOC
{
    public static class SLWIOC
    {
        private static IDictionary<Type, Object> _registeredInstances = new Dictionary<Type, Object>();

        private static IDictionary<Type, Type> _registeredTypes = new Dictionary<Type, Type>();

        public static void Register<I>(Object instance)
        {
            _registeredInstances.Add(typeof(I), instance);
        }

        public static I Get<I>() where I : class
        {
            return _registeredInstances[typeof(I)] as I;
        }

        public static void Register<I>(Type instanceType)
        {
            _registeredTypes.Add(typeof(I), instanceType);
        }

        public static I Create<I>() where I : class
        {
            var type = _registeredTypes[typeof(I)];
            return Activator.CreateInstance(type) as I;
        }

        public static bool Contains(Type type)
        {
            return _registeredTypes.ContainsKey(type);
        }
    }
}
