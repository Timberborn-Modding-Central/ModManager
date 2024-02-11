using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModManager;
using Timberborn.SingletonSystem;

namespace ModManagerUI.EventSystem
{
    public class EventBus : Singleton<EventBus>
    {
        private readonly List<EventListenerWrapper> _listeners = new();
        private readonly BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        
        public void Register(object listener)
        {
            if (_listeners.Any(wrapper => wrapper.Listener == listener)) 
                return;
            foreach (var method in listener.GetType().GetMethods(_bindingFlags).Where(method => method.GetCustomAttributes(typeof(OnEventAttribute)).Any()))
            {
                _listeners.Add(new EventListenerWrapper(listener, method));
            }
        }

        public void Unregister(object listener)
        {
            _listeners.RemoveAll(eventListenerWrapper => eventListenerWrapper.Listener == listener);
        }

        public void PostEvent(object @event)
        {
            _listeners.Where(eventListenerWrapper => eventListenerWrapper.EventType == @event.GetType()).ToList().ForEach(eventListenerWrapper => eventListenerWrapper.PostEvent(@event));
        }
        
        private class EventListenerWrapper
        {
            private readonly MethodBase method;
            
            public object Listener { get; }
            public Type EventType { get; }

            public EventListenerWrapper(object listener, MethodInfo methodInfo)
            {
                Listener = listener;

                var type = listener.GetType();

                method = methodInfo;
                if (method == null)
                    throw new ArgumentException("Class " + type.Name + " does not contain method OnEvent.");

                var parameters = method.GetParameters();
                if (parameters.Length != 1)
                    throw new ArgumentException("Method OnEvent of class " + type.Name + " have invalid number of parameters (should be one).");

                EventType = parameters[0].ParameterType;
            }

            public void PostEvent(object e)
            {
                method.Invoke(Listener, new[] { e });
            }
        }
    }
}