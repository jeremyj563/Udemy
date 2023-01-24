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
    public static void Main(string[] args)
    {
        var cb = new ContainerBuilder();
        cb.RegisterType<Service>();
        cb.RegisterType<DomainObject>();

        var container = cb.Build();

        // naive approach to setting a specific parameter at resolution time
        // will break if parameter postion changes
        var domainObject = container.Resolve<DomainObject>(new PositionalParameter(1, 42));
        Console.WriteLine(domainObject.ToString());

        // use a delegate factore to set the parameter at resolution time
        // will *not* break
        var factory = container.Resolve<DomainObject.Factory>();
        domainObject = factory(23); // notice the domainObject still get's the Service injected...
        Console.WriteLine(domainObject.ToString());
    }
}