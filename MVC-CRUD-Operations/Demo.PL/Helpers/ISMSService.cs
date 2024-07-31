using Demo.DAL.Models;
using Twilio.Rest.Api.V2010.Account;

namespace Demo.PL.Helpers
{
    public interface ISMSService
    {
        public MessageResource Send(SMSMessage sms);
    }
}
