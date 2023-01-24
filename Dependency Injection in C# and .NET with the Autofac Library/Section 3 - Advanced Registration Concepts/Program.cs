using Autofac;
using Autofac.Core;

public interface ILog
{
    void Write(string message);
}

public class ConsoleLog : ILog
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}

public class EmailLog : ILog
{
    private const string adminEmail = "admin@foo.com";
    public void Write(string message)
    {
        Console.WriteLine($"Email sent to {adminEmail} : {message}");
    }
}

public class SMSLog : ILog
{
    public string phoneNumber { init; get; }

    public SMSLog(string phoneNumber)
    {
        this.phoneNumber = phoneNumber;
    }

    public void Write(string message)
    {
        Console.WriteLine($"SMS to {phoneNumber} : {message}");
    }
}

public class Engine
{
    private ILog log;
    private int id;

    public Engine(ILog log)
    {
        this.log = log;
        this.id = new Random().Next();
    }

    public void Ahead(int power)
    {
        log.Write($"Engine [{id}] ahead {power}");
    }
}

public class Car
{
    private Engine engine;
    private ILog log;

    public Car(Engine engine, ILog log)
    {
        this.engine = engine;
        this.log = log;
    }

    public void Go()
    {
        this.engine.Ahead(100);
        this.log.Write("Car going forward...");
    }
}

internal class Program
{
    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();

        // *** PROVIDE ARGUMENT AT REGISTRATION TIME ***

        // named parameter
        builder.RegisterType<SMSLog>()
               .As<ILog>()
               .WithParameter(nameof(SMSLog.phoneNumber), "+12345678");

        // typed parameter
        builder.RegisterType<SMSLog>()
               .As<ILog>()
               .WithParameter(new TypedParameter(typeof(string), "+12345678"));

        // resolved parameter
        builder.RegisterType<SMSLog>()
               .As<ILog>()
               .WithParameter(
                new ResolvedParameter(
                    // predicate - resolve the parameter by type and name
                    (p, c) => p.ParameterType == typeof(string) && p.Name == nameof(SMSLog.phoneNumber),
                    // valueAccessor - specify the value to pass as argument to the above resolved parameter
                    (p, c) => "+12345678"
                ));

        // Console.WriteLine("About to build container...");
        // var container = builder.Build();

        // var log = container.Resolve<ILog>();
        // log.Write("static phoneNumber set at registration time");

        // *** PROVIDE ARGUMENT AT RESOLUTION TIME ***

        var random = new Random();
        builder.Register(
            // delegate - specify parameters to be set at resolution time
            (c, p) => new SMSLog(p.Named<string>(nameof(SMSLog.phoneNumber)))
            ).As<ILog>();

        Console.WriteLine("About to build container...");
        var container = builder.Build();

        var log = container.Resolve<ILog>(
            // set the parameter at resolution time
            new NamedParameter(
                nameof(SMSLog.phoneNumber),
                random.Next().ToString()
            ));
        log.Write("dynamic phoneNumber set at resolution time");
    }
}