using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Microsoft.Maui.Controls.PlatformConfiguration;
using SmsScheduler;

namespace SmsScheduler.Platforms.Android
{
    [Service(
         Enabled = true,
         Exported = false,
         ForegroundServiceType = ForegroundService.TypeDataSync
     )]
    public class SmsForegroundService : Service
    {
        private Timer? _timer;

        private void startForegroundService()
        {
            var channelId = "sms_service_channel";
            var channel = new NotificationChannel(channelId, "SMS Service", NotificationImportance.Default);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            var notification = new NotificationCompat.Builder(this, channelId)
                .SetContentTitle("SMS Scheduler")
                .SetContentText("Running...")
                .SetSmallIcon(global::Android.Resource.Drawable.IcDialogInfo)
                .Build();

            StartForeground(1, notification, ForegroundService.TypeDataSync);
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            startForegroundService();
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            _timer = new Timer(
                 callback: state =>
                 {
                     SmsHandler.CheckPeriodicalSms();
                 },
                 state: null,
                 dueTime: TimeSpan.Zero,
                 period: TimeSpan.FromMinutes(1)
             );
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _timer?.Dispose();
        }

    }
}
