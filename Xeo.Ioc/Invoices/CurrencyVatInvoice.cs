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
            // zrobić z currencySymbol klasę, która waliduje symbol
            string currencySymbol) 
            : base(net, taxRate)
            => _currencySymbol = currencySymbol;

        // dodać GetValueAsync do interfejsu, usunąć synchroniczną
        public override decimal GetValue()
            => GetValueAsync()
                .GetAwaiter()
                .GetResult();

        public async Task<decimal> GetValueAsync()
        {
            // 1. HttpClient nie może być tworzony w ten sposób, ponieważ będzie tworzył wycieki. Trzeba cacheować, lub korzystać z IHttpClientFactory
            // 2. Wyekstraktować logikę na zewnątrz
            var response = await new HttpClient().GetAsync($"https://nbp.pl/rates/{_currencySymbol}/pln");

            if (response.IsSuccessStatusCode)
            {
                // brak zamknięcia strumienia, wycieki
                Stream stream 
                    = await response.Content.ReadAsStreamAsync();
                decimal exchangeRate 
                    = Convert.ToDecimal(await JsonSerializer.DeserializeAsync<string>(stream));

                // brak walidacji danych przychodzących
                
                return base.GetValue() * exchangeRate;
            }
            
            // błąd zbyt ogólny, dodać klasę wyjątku ExchangeRateLoadingException
            throw new Exception("Could not get the exchange rate.");
        }
    }
}