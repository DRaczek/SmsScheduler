#if ANDROID
using SmsScheduler.Platforms.Android;
using System.ComponentModel.DataAnnotations;
#endif

namespace SmsScheduler
{
    public partial class MainPage : ContentPage
    {

        public List<Group> Groups { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            UpdateView();
        }

        protected override async void OnAppearing()
        {
            await UpdateView();

            if(await AndroidPermissionUtils.CheckPermissionSms() is not null and string error)
            {
                await DisplayAlert("Błąd", $"{error} Aplikacja nie będzie działać poprawnie", "OK");
            }

            StartForegroundService();

            base.OnAppearing();
        }

        private void StartForegroundService()
        {
#if ANDROID
            Android.Content.Intent intent = new Android.Content.Intent(Android.App.Application.Context, typeof(SmsForegroundService));
            Android.App.Application.Context.StartForegroundService(intent);
#endif
        }

        private async Task UpdateView()
        {
            Groups = await App.Database.GetGroupsAsync();
            GroupsCollection.ItemsSource = Groups;
        }

        private async void OnAddGroupClicked(object sender, EventArgs e)
        {
            var name = GroupNameEntry.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("Błąd", "Podaj nazwę grupy.", "OK");
                return;
            }

            var newGroup = new Group { Name = name };
            await App.Database.SaveGroupAsync(newGroup);

            GroupNameEntry.Text = string.Empty;
            await UpdateView();
        }

        private async void OnDeleteGroup(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipe && swipe.BindingContext is Group group)
            {
                bool confirm = await DisplayAlert("Potwierdzenie", $"Usunąć grupę '{group.Name}'?", "Tak", "Nie");
                if (confirm)
                {
                    await App.Database.DeleteGroupAsync(group);
                    await UpdateView();
                }
            }
        }

        private async void OnEditGroup(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipe && swipe.BindingContext is Group group)
            {
                await Navigation.PushAsync(new EditGroupPage(group));
            }
        }

        private async void OnSendSms(object sender, EventArgs e)
        {
            if (sender is SwipeItem swipe && swipe.BindingContext is Group group)
            {
                await Navigation.PushAsync(new SensSmsPage(group));
            }
        }

        private async void onDelayedSmsPageClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DelayedSmsesPage());
        }
    }

}
