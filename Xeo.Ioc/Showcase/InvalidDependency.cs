using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xeo.Ioc.Showcase
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class Dependency : Attribute
    {
        public IEnvironment Environment { get; }
        
        public Dependency(Type environmentType)
        {
            if (environmentType == null)
            { 
                throw new ArgumentNullException(nameof(environmentType));   
            }
            
            Environment = (IEnvironment)Activator.CreateInstance(environmentType);
        }
    }

    public static class Environments
    {
        public static IEnvironment Development { get; } = new DevelopmentEnvironment();
        public static IEnvironment Production { get; } = new ProductionEnvironment();

        public static IEnvironment Get(Type environmentType)
            => (IEnvironment)typeof(Environments)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.PropertyType == typeof(IEnvironment))
                .Single(x => x
                        .GetValue(null)
                        .GetType() ==
                    environmentType)
                .GetValue(null);
    }

    public interface IEnvironment { }

    public class DevelopmentEnvironment : IEnvironment { }
    public class ProductionEnvironment : IEnvironment { }

    public interface IInvoiceProcessor { }
    
    [Showcase.Dependency(typeof(ProductionEnvironment))]
    public class ProductionInvoiceProcessor : IInvoiceProcessor { }

    [Showcase.Dependency(typeof(DevelopmentEnvironment))]
    public class DevelopmentInvoiceProcessor : IInvoiceProcessor { }

    public class SomewhereInTheCode
    {
        public void Method()
        {
            IEnvironment environment = new DevelopmentEnvironment();
            
            IInvoiceProcessor invoiceProcessor = new ProductionInvoiceProcessor();

            if (invoiceProcessor.GetType()
                    .GetCustomAttribute<Dependency>()
                    .Environment !=
                environment)
            {
                throw new Exception("Invalid dependency for current environment.");
            }
        }
    }
}