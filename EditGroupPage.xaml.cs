using Microsoft.Maui.ApplicationModel.Communication;
namespace SmsScheduler;

public partial class EditGroupPage : ContentPage
{
    private Group _group;
    private List<Contact> contacts = new();

    public EditGroupPage(Group group)
    {
        InitializeComponent();
        _group = group;
        GroupNameEntry.Text = _group.Name;
        UpdateView();
    }

    private async void UpdateView()
    {
        contacts = await App.Database.GetContactsForGroupAsync(_group.Id);
        ContactsCollection.ItemsSource = contacts;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var newName = GroupNameEntry.Text?.Trim();
        if (string.IsNullOrEmpty(newName))
        {
            await DisplayAlert("B³¹d", "Nazwa nie mo¿e byæ pusta.", "OK");
            return;
        }

        _group.Name = newName;
        await App.Database.UpdateGroupAsync(_group);

        await DisplayAlert("OK", "Zapisano zmiany.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnAddContactClicked(object sender, EventArgs e)
    {
        try
        {
            if(await AndroidPermissionUtils.CheckPermissionReadContacts() is not null and string permissonError)
            {
                await DisplayAlert("B³¹d", permissonError, "OK");
            }

            var phoneContact = new Microsoft.Maui.ApplicationModel.Communication.Contact();
#if ANDROID
            phoneContact = await Contacts.Default.PickContactAsync();

            if(Validate(phoneContact) is not null and string error)
            {
                await DisplayAlert("B³¹d", error, "OK");
                return;
            }

            //contact exists in the database
            var phoneNumber = phoneContact.Phones?.FirstOrDefault()?.PhoneNumber;
            var dbContact = (await App.Database.GetContactsAsync()).FirstOrDefault(c => c.PhoneNumber == phoneNumber);

            if (dbContact == null)
            {
                dbContact = new Contact
                {
                    Name = phoneContact.DisplayName,
                    PhoneNumber = phoneNumber
                };
                await App.Database.SaveContactAsync(dbContact);
            }

            //contact is already assigned to the group
            var existing = await App.Database.GetGroupContactAsync(_group.Id, dbContact.Id);
            if (existing == null)
            {
                await App.Database.SaveGroupContactAsync(new GroupContact
                {
                    GroupId = _group.Id,
                    ContactId = dbContact.Id
                });
            }

            UpdateView();
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê dodaæ kontaktu: {ex.Message}", "OK");
        }
#endif
    }

    private string? Validate(Microsoft.Maui.ApplicationModel.Communication.Contact contact)
    {
        //contact picker cancelled
        if (contact == null)
        {
            return "Wyst¹pi³ nieoczekiwany b³¹d";
        };

        //contact isn't a phone number
        var phone = contact.Phones?.FirstOrDefault()?.PhoneNumber;
        if (string.IsNullOrWhiteSpace(phone))
        {
            return "Wybrany kontakt nie ma numeru telefonu.";
        }
        return null;
    }

    private async void OnDeleteContact(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipe && swipe.BindingContext is Contact contact)
        {
            var groupContact = await App.Database.GetGroupContactAsync(_group.Id, contact.Id);
            if (groupContact != null)
            {
                await App.Database.DeleteGroupContactAsync(groupContact);
                UpdateView();
            }
        }
    }
}