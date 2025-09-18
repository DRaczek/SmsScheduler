#if ANDROID
using Android.Telephony;
using Android.Content;
using Android.App;
using Android.Service.Controls.Actions;
#endif


namespace SmsScheduler;

public partial class SensSmsPage : ContentPage
{
    private Group _group;

    public SensSmsPage(Group group)
    {
        InitializeComponent();
        _group = group;
    }

    private async void OnSendSmsClicked(object sender, EventArgs e)
    {
        if (await AndroidPermissionUtils.CheckPermissionSms() is not null and string smsPermissionError)
        {
            await DisplayAlert("B��d", smsPermissionError, "OK");
        }

        if (await CreateEntityIfDelayed() == true) return;

        if (await GetMessage() is not null and string message) { } else return;

#if ANDROID
        try
        {
            if (await SmsHandler.SendSms(_group, message) is not null and string SmsSendError)
            {
                await DisplayAlert("B��d", "Wyst�pi� nieoczekiwany b��d : " + SmsSendError, "OK");
                return;
            }

            await DisplayAlert("OK", "SMS wys�any do grupy.", "OK");
        }
        catch (System.Exception ex)
        {
            await DisplayAlert("B��d", $"Nie uda�o si� wys�a� SMS: {ex.Message}", "OK");
        }
#endif
    }

    private async Task<bool> CreateEntityIfDelayed()
    {
        if (isDelayedChck.IsChecked != true) return false;

        DateTime dateTime = GetDateTime();
        DelayedSms sms = new DelayedSms()
        {
            Message = MessageEditor.Text?.Trim() ?? "",
            DateTime = dateTime,
            GroupId = _group.Id
        };
        await App.Database.SaveDalayedSms(sms);
        await DisplayAlert("Zapisano", $"SMS zostanie wys�any {dateTime}", "OK");
        await Navigation.PopAsync();
        return true;
    }

    private DateTime GetDateTime()
    {
        return new DateTime(DatePicker.Date.Year, DatePicker.Date.Month, DatePicker.Date.Day, TimePicker.Time.Hours, TimePicker.Time.Minutes, TimePicker.Time.Seconds);
    }

    private async Task<string?> GetMessage()
    {
        var message = MessageEditor.Text?.Trim();
        if (string.IsNullOrEmpty(message))
        {
            await DisplayAlert("B��d", "Wpisz wiadomo��.", "OK");
            return null;
        }
        return message;
    }
}