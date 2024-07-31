using Demo.DAL.Models;

namespace Demo.PL.Helpers
{
    public interface IMailSettings
    {
        public void SendMail(Email email);
    }

}
