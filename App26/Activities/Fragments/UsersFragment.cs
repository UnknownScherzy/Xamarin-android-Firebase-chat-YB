using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using App26.AppDataHelpers;
using App26.Entity;
using Firebase.Firestore;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App26
{
    /// <summary>
    /// Creates a view with list of users.
    /// Every item consist of name, profile photo and email. 
    /// </summary>
    public class UsersFragment : Fragment, IChat, ISelectable
    {
        private RecyclerView _recyclerView;
        private PreviewAdapter _usersAdapter;
        private List<EntityPreview> _userPreviews;
        private ImageView _createGroupButton;


        public UsersFragment(ImageView createGroupButton)
        {
            _createGroupButton = createGroupButton;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.users_fragment, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.usersRecycleView);
            _userPreviews = new List<EntityPreview>();
            _usersAdapter = new PreviewAdapter(_userPreviews, Context, this, this);
            _recyclerView.SetAdapter(_usersAdapter);

            if (!_createGroupButton.HasOnClickListeners)
            {
                _createGroupButton.Click += async delegate
                {
                    await CreateGroup();
                };
            }

            await LoadUsers();
        }

        private async Task CreateGroup()
        {
            await FirebaseDataHelper.Database
                    .Collection(Constants.CONVERSATION_TABLE_ID)
                    .Add(CreateConversation("Vítejte ve skupinové konverzaci"))
                    .AsAsync();
        }

        private HashMap CreateConversation(string inputMessage)
        {
            HashMap conversation = new();
            conversation.Put(Constants.PARTICIPANTS, CreateParticipants());
            conversation.Put(Constants.LAST_MESSAGE, inputMessage);
            conversation.Put(Constants.MESSAGE_DATE, DateTime.Now.FormatDate());
            conversation.Put(Constants.GROUP_PHOTO, "");
            conversation.Put(Constants.GROUP_NAME, "Skupinová konverzace");
            conversation.Put(Constants.IS_GROUP, true);

            return conversation;
        }

        private ArrayList CreateParticipants()
        {
            ArrayList participants = new();
            DocumentReference documentReference;
            foreach (EntityPreview entityPreview in _userPreviews)
            {
                if (entityPreview.Selected)
                {
                    documentReference = FirebaseDataHelper.Database
                                            .Collection(Constants.USERS_TABLE_ID)
                                            .Document(entityPreview.EntityId);
                    participants.Add(documentReference);
                }
            }
            documentReference = FirebaseDataHelper.Database
                                     .Collection(Constants.USERS_TABLE_ID)
                                     .Document(LocalDatabase.GetString(Constants.USER_ID));
            participants.Add(documentReference);
            return participants;
        }

        private async Task LoadUsers()
        {
            QuerySnapshot result = await FirebaseDataHelper.Database.
                Collection(Constants.USERS_TABLE_ID).
                Get().
                AsAsync<QuerySnapshot>();

            string loggedUserId = LocalDatabase.GetString(Constants.USER_ID);

            foreach (QueryDocumentSnapshot queryDocumentSnapshot in result.Documents)
            {
                if (queryDocumentSnapshot.Id == loggedUserId)
                {
                    continue;
                }

                EntityPreview userPreview = new()
                {
                    Name = queryDocumentSnapshot.GetString(Constants.USER_NAME),
                    PreviewText = queryDocumentSnapshot.GetString(Constants.USER_EMAIL),
                    EntityId = queryDocumentSnapshot.Id,
                    ProfilePhoto = queryDocumentSnapshot.GetString(Constants.USER_PHOTO),
                };

                _userPreviews.Add(userPreview);
            }

            _usersAdapter.NotifyDataSetChanged();
        }

        public void OpenChat(EntityPreview entityPreview)
        {
            Intent intent = new(Context, typeof(ChatActivity));
            intent.PutExtra(Constants.USER_ID, JsonConvert.SerializeObject(entityPreview));
            StartActivity(intent);
        }

        public void OnSelectionStarted()
        {
            _createGroupButton.Visibility = ViewStates.Visible;
        }

        public void OnSelectionFinished()
        {
            _createGroupButton.Visibility = ViewStates.Gone;

            foreach (EntityPreview entityPreview in _userPreviews)
            {
                entityPreview.Selected = false;
            }
        }

        public void Select(EntityPreview entityPreview)
        {
            entityPreview.Selected = !entityPreview.Selected;
        }
    }
}