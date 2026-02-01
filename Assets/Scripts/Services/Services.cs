using System;
using System.Collections.Generic;

public static class Services
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>()
    {
        return (T)_services[typeof(T)];
    }

    public static void  Unregister<T>()
    {
        _services.Remove(typeof(T));
    }
}
