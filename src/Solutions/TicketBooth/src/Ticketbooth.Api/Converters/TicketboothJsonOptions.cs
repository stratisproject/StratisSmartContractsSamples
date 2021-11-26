using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Ticketbooth.Api.Converters
{
    public class TicketboothJsonOptions : IConfigureOptions<MvcJsonOptions>
    {
        private readonly AddressConverter _addressConverter;
        private readonly ByteArrayToHexConverter _byteArrayToHexConverter;

        public TicketboothJsonOptions(AddressConverter addressConverter, ByteArrayToHexConverter byteArrayToHexConverter)
        {
            _addressConverter = addressConverter;
            _byteArrayToHexConverter = byteArrayToHexConverter;
        }

        public void Configure(MvcJsonOptions options)
        {
            options.SerializerSettings.Converters.Add(_addressConverter);
            options.SerializerSettings.Converters.Add(_byteArrayToHexConverter);
        }
    }
}
