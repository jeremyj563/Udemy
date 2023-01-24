using Autofac;
using Autofac.Core;

public class Service
{
    public string DoSomething(int value)
    {
        return $"I have {value}";
    }
}

public class DomainObject
{
    private Service service;
    private int value;

    public delegate DomainObject Factory(int value);

    public DomainObject(Service service, int value)
    {
        this.service = service;
        this.value = value;
    }

    public override string ToString()
    {
        return this.service.DoSomething(this.value);
    }
}

public class Demo
{

    public delegate DomainObject Factory(int value);

    public static DomainObject CreateDomainObject(int value)
    {
        return new DomainObject(new Service(), value);
    }

    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<Service>();
        builder.RegisterType<DomainObject>();

        var container = builder.Build();

        // naive approach to setting a specific parameter at resolution time
        // will break if parameter postion changes
        var domainObject = container.Resolve<DomainObject>(new PositionalParameter(1, 42));
        Console.WriteLine(domainObject.ToString());

        // use a delegate factore to set the parameter at resolution time
        // will *not* break
        var factory = container.Resolve<DomainObject.Factory>();
        domainObject = factory(23); // notice the domainObject still get's the Service injected...
        Console.WriteLine(domainObject.ToString());

        // playing around with delegate options...
        Factory factoryOuterDelegate = CreateDomainObject;
        domainObject = factoryOuterDelegate(23);

        Factory factoryLocalDelegate = v => new DomainObject(new Service(), v);
        domainObject = factoryLocalDelegate(23);

        var factoryInlineDelegate = delegate (int v) { return new DomainObject(new Service(), v); };
        domainObject = factoryInlineDelegate(23);

        var factoryFunc = (int v) => new DomainObject(new Service(), v);
        domainObject = factoryFunc(23);

        // should be able to just pass a lambda like ^^^ when calling builder.Resolve();
    }
}