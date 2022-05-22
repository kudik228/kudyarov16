using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;


namespace EnukhinPr16
{
    class ApplicationViewModel
    {
        bool initialized = false;   // была ли начальная инициализация
        Friend selectedFriend;  // выбранный друг
        private bool isBusy;    // идет ли загрузка с сервера

        public ObservableCollection<Friend> Friends { get; set; }
        FriendsService friendsService = new FriendsService();
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CreateFriendCommand { protected set; get; }
        public ICommand DeleteFriendCommand { protected set; get; }
        public ICommand SaveFriendCommand { protected set; get; }
        public ICommand BackCommand { protected set; get; }

        public INavigation Navigation { get; set; }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
                OnPropertyChanged("IsLoaded");
            }
        }
        public bool IsLoaded
        {
            get { return !isBusy; }
        }

        public ApplicationViewModel()
        {
            Friends = new ObservableCollection<Friend>();
            IsBusy = false;
            CreateFriendCommand = new Command(CreateFriend);
            DeleteFriendCommand = new Command(DeleteFriend);
            SaveFriendCommand = new Command(SaveFriend);
            BackCommand = new Command(Back);
        }

        public Friend SelectedFriend
        {
            get { return selectedFriend; }
            set
            {
                if (selectedFriend != value)
                {
                    Friend tempFriend = new Friend()
                    {
                        Id = value.Id,
                        Name = value.Name,
                        Email = value.Email,
                        Phone = value.Phone
                    };
                    selectedFriend = null;
                    OnPropertyChanged("SelectedFriend");
                    Navigation.PushAsync(new FriendPage(tempFriend, this));
                }
            }
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private async void CreateFriend()
        {
            await Navigation.PushAsync(new FriendPage(new Friend(), this));
        }
        private void Back()
        {
            Navigation.PopAsync();
        }

        public async Task GetFriends()
        {
            if (initialized == true) return;
            IsBusy = true;
            IEnumerable<Friend> friends = await friendsService.Get();

            // очищаем список
            //Friends.Clear();
            while (Friends.Any())
                Friends.RemoveAt(Friends.Count - 1);

            // добавляем загруженные данные
            foreach (Friend f in friends)
                Friends.Add(f);
            IsBusy = false;
            initialized = true;
        }
        private async void SaveFriend(object friendObject)
        {
            Friend friend = friendObject as Friend;
            if (friend != null)
            {
                IsBusy = true;
                // редактирование
                if (friend.Id > 0)
                {
                    Friend updatedFriend = await friendsService.Update(friend);
                    // заменяем объект в списке на новый
                    if (updatedFriend != null)
                    {
                        int pos = Friends.IndexOf(updatedFriend);
                        Friends.RemoveAt(pos);
                        Friends.Insert(pos, updatedFriend);
                    }
                }
                // добавление
                else
                {
                    Friend addedFriend = await friendsService.Add(friend);
                    if (addedFriend != null)
                        Friends.Add(addedFriend);
                }
                IsBusy = false;
            }
            Back();
        }
        private async void DeleteFriend(object friendObject)
        {
            Friend friend = friendObject as Friend;
            if (friend != null)
            {
                IsBusy = true;
                Friend deletedFriend = await friendsService.Delete(friend.Id);
                if (deletedFriend != null)
                {
                    Friends.Remove(deletedFriend);
                }
                IsBusy = false;
            }
            Back();
        }

    }
}
