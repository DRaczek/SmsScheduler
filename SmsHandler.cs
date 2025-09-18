#if ANDROID
using Android.Telephony;
using Android.Content;
using Android.App;
#endif


namespace SmsScheduler
{
    public static class SmsHandler
    {
        public static async void CheckPeriodicalSms()
        {
            List<DelayedSms> delayedSms = await App.Database.GetDalayedSmses();
            foreach(DelayedSms sms in delayedSms)
            {
                DateTime date = sms.DateTime;
                if(date.Year == DateTime.Now.Year
                    && date.Month == DateTime.Now.Month
                    && date.Day == DateTime.Now.Day
                    && date.Hour == DateTime.Now.Hour
                    && Math.Abs( date.Minute - DateTime.Now.Minute) <=0)
                {
                    await SendSms(await App.Database.GetGroupAsync(sms.GroupId), sms.Message);
                    await App.Database.DeleteDelayedSms(sms);
                }
            }

        }

        public static async Task<string?> SendSms(Group group, string message)
        {
            try
            {
                var contacts = await App.Database.GetContactsForGroupAsync(group.Id);
                var smsManager = SmsManager.Default;

                foreach (var contact in contacts)
                {
                    if (!string.IsNullOrWhiteSpace(contact.PhoneNumber))
                        smsManager.SendTextMessage(contact.PhoneNumber, null, message, null, null);
                }
                
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
    }
}
