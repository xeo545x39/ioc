using System;
using System.Linq;
using System.Reflection;

namespace Xeo.Ioc.Showcase
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal sealed class DependencyAttribute : Attribute
    {
        public DependencyAttribute(Type environmentType)
        {
            if (environmentType == null)
            {
                throw new ArgumentNullException(nameof(environmentType));
            }

            Environment = Environments.Get(environmentType);
        }

        public IEnvironment Environment { get; }
    }

    public static class Environments
    {
        public static IEnvironment Development { get; } = new DevelopmentEnvironment();
        public static IEnvironment Production { get; } = new ProductionEnvironment();

        public static IEnvironment Get(Type environmentType)
            => (IEnvironment) typeof(Environments)
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

    [Dependency(typeof(ProductionEnvironment))]
    public class ProductionInvoiceProcessor : IInvoiceProcessor { }

    [Dependency(typeof(DevelopmentEnvironment))]
    public class DevelopmentInvoiceProcessor : IInvoiceProcessor { }

    public class SomewhereInTheCode
    {
        public void Method()
        {
            IEnvironment environment = new DevelopmentEnvironment();

            IInvoiceProcessor invoiceProcessor = new ProductionInvoiceProcessor();

            if (invoiceProcessor.GetType()
                    .GetCustomAttribute<DependencyAttribute>()
                    .Environment !=
                environment)
            {
                throw new Exception("Invalid dependency for current environment.");
            }
        }
    }
}