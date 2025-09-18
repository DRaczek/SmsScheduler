using Android.Text.Style;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmsScheduler;

public partial class DelayedSmsesPage : ContentPage
{
    public ObservableCollection<DelayedSms> smses { get; set; }

    public DelayedSmsesPage()
    {
        InitializeComponent();
        UpdateView();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        UpdateView();
        base.OnAppearing();
    }
    private async void UpdateView()
    {
        List<DelayedSms> smsList = await App.Database.GetDalayedSmses();
        foreach (var sms in smsList)
        {
            sms.Contacts = await App.Database.GetContactsForGroupAsync(sms.GroupId);
        }
        smses = new ObservableCollection<DelayedSms>(smsList);
        SmsCollectionView.ItemsSource = smses;
    }

    private async void OnDelete(DelayedSms sms)
    {
        await App.Database.DeleteDelayedSms(sms);
        UpdateView();
    }

    private async void OnShowMessage(DelayedSms sms)
    {
        string receivers = "";
        foreach(Contact contact in sms.Contacts)
        {
            receivers+= $"{contact.Name}, {contact.PhoneNumber}\n";
        }
        await DisplayAlert("Treœæ wiadomoœci", $"{receivers}{sms.Message}", "OK");
    }

    private void SwipeItem_Clicked(object sender, EventArgs e)
    {
        DelayedSms sms = (DelayedSms)(sender as SwipeItem).BindingContext;
        OnDelete(sms);
    }

    private void SwipeItem_Clicked_1(object sender, EventArgs e)
    {
        DelayedSms sms = (DelayedSms)(sender as SwipeItem).BindingContext;
        OnShowMessage(sms);
    }
}