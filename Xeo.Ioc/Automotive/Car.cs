﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

public class Wheel { }

public interface IInterior { }

public interface IEngine { }

public interface IWheel { }



public interface ICarDependencies
{
    IEngine Engine { get; }
    IEnumerable<IWheel> Wheels { get; }
    IInterior Interior { get; }
}

public class CarDependencies : ICarDependencies
{
    public CarDependencies(IEngine engine, IEnumerable<IWheel> wheels, IInterior interior)
    {
        Engine = engine;
        Wheels = wheels;
        Interior = interior;
    }
    
    // tutaj przydaje się wstrzykiwanie przez właściwość, względnie bezpiecznie, bo przypisujemy wszystko od razu
    public IEngine Engine { get; }
    public IEnumerable<IWheel> Wheels { get; }
    public IInterior Interior { get; }
}

public interface ISafeService<T>
{
    void Add(T @object);
    IEnumerator<T> GetEnumerator();
}

public class NotSafeService<T> : ISafeService<T>
{
    private ICollection<T> _collection;

    public NotSafeService()
        => _collection = new List<T>();


    public void Add(T @object)
        => _collection.Add(@object);

    public IEnumerator<T> GetEnumerator()
        => _collection.GetEnumerator();
}

public class DieselEngine : IEngine { }

public interface IEngineInjectable
{
    void SetEngine(IEngine engine);
}

public interface ICar { }
public class Car : ICar
{
    private IEngine Engine { get; set; }

    public Car(IEngine engine, ILifetimeScope scope) // implicit resolve via constructor
    {
        var sameEngine = DI.Container.Resolve<IEngine>(); // explicit resolve via Service Locator pattern
        var anotherSameEngine = scope.Resolve<IEngine>(); // explicit resolve via Service Locator, but from injected scope`
    }
}

public interface ILifetimeScope {
    object Resolve<T>();
}

public static class DI
{
    public static class Container
    {
        public static T Resolve<T>()
            => default;
    }

}

public static class ObjectExtensions
{
    public static Type GetType<T>(this object @this, Expression<Func<T>> expression)
        => expression.Body.Type;
}