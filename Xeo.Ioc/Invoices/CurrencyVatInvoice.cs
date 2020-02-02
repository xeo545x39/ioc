using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Xeo.Ioc.Invoices
{
    public class CurrencyVatInvoice : VatInvoice
    {
        private readonly string _currencySymbol;
        
        public CurrencyVatInvoice(
            decimal net, 
            decimal taxRate, 
            string currencySymbol) 
            : base(net, taxRate)
            => _currencySymbol = currencySymbol;

        public override decimal GetValue()
            => GetValueAsync()
                .GetAwaiter()
                .GetResult();

        public async Task<decimal> GetValueAsync()
        {
            var response = await new HttpClient().GetAsync($"https://nbp.pl/rates/{_currencySymbol}/pln");

            if (response.IsSuccessStatusCode)
            {
                Stream stream 
                    = await response.Content.ReadAsStreamAsync();
                decimal exchangeRate 
                    = Convert.ToDecimal(await JsonSerializer.DeserializeAsync<string>(stream));

                return base.GetValue() * exchangeRate;
            }
            
            throw new Exception("Could not get the exchange rate.");
        }
    }
}