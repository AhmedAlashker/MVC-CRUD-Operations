using Demo.DAL.Models;
using Demo.PL.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helpers
{
    public class SMSService : ISMSService
    {
        private TwilioSettings _options;

        public SMSService(IOptions<TwilioSettings> options)
        {
            _options = options.Value;
        }

        public MessageResource Send(SMSMessage sms)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);

            var Result = MessageResource.Create(
                body: sms.Body,
                from: new Twilio.Types.PhoneNumber(_options.TwilioPhoneNumber),
                to: sms.PhoneNumber
                );

            return Result;
        }
    }
}
