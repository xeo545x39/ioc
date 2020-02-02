using Xeo.Ioc.Invoices.Abstract;

namespace Xeo.Ioc.Invoices
{
    public class VatInvoice : IInvoice
    {
        private readonly decimal _net;
        private readonly decimal _taxRate;
  
        public VatInvoice(decimal net, decimal taxRate)
        {
            _net = net;
            _taxRate = taxRate;
        }
      
        public virtual decimal GetValue()
            => _net * (1 + _taxRate);
    } 
}