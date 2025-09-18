using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsScheduler
{
    public static class AndroidPermissionUtils
    {
        public static async Task<string?> CheckPermissionSms()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Sms>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Sms>();
            }
            if (status != PermissionStatus.Granted)
            {
                return "Brak uprawnień do wysyłania SMS przez aplikację. ";
                
            }
            return null;
        }

        public static async Task<string?> CheckPermissionReadContacts()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            }
            if (status != PermissionStatus.Granted)
            {
                return "Brak uprawnień dostępu do listy kontaktów w telefonie. ";

            }
            return null;
        }
    }
}
