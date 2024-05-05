using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using App26.AppDataHelpers;
using Firebase.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App26
{
    public class ChatsFragment : Fragment, IChat, IEventListener
    {
        private List<EntityPreview> _chatPreviews;

        private PreviewAdapter _chatsAdapter;
        private RecyclerView _recyclerView;

        private DocumentReference _referenceDocument;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.chats_fragment, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            _recyclerView = View.FindViewById<RecyclerView>(Resource.Id.chatsRecycleView);
            _chatPreviews = new List<EntityPreview>();

            _chatsAdapter = new PreviewAdapter(_chatPreviews, Context, this);
            _recyclerView.SetAdapter(_chatsAdapter);

            _referenceDocument = FirebaseDataHelper.Database
                .Collection(Constants.USERS_TABLE_ID)
                .Document(LocalDatabase.GetString(Constants.USER_ID));

            ChatsListener();
        }

        private void ChatsListener()
        {
            FirebaseDataHelper.Database
                  .Collection(Constants.CONVERSATION_TABLE_ID)
                  .WhereArrayContains(Constants.PARTICIPANTS, _referenceDocument)
                  .AddSnapshotListener(this);
        }

        public async void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            if (error != null || obj == null)
            {
                return;
            }

            if (obj is not QuerySnapshot querySnapshot)
            {
                return;
            }

            foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
            {
                if ((bool)documentChange.Document.GetBoolean(Constants.IS_GROUP))
                {
                    LoadGroupConversation(documentChange);
                }
                else
                {
                    await LoadBasicConversation(documentChange);
                }
            }

            _chatPreviews.Sort((x, y) => y.LastChangeTime.CompareTo(x.LastChangeTime));
            _chatsAdapter.NotifyDataSetChanged();
        }

        private void LoadGroupConversation(DocumentChange documentChange)
        {
            if (!TryUpdate(documentChange))
            {
                EntityPreview userPreview = new()
                {
                    Name = documentChange.Document.GetString(Constants.GROUP_NAME),
                    ProfilePhoto = documentChange.Document.GetString(Constants.GROUP_PHOTO),
                    PreviewText = documentChange.Document.GetString(Constants.LAST_MESSAGE),
                    LastChangeTime = documentChange.Document.GetString(Constants.MESSAGE_DATE).FormatStringToDate(),
                    ConversationId = documentChange.Document.Id,
                    IsGroup = true,
                };

                _chatPreviews.Add(userPreview);
            }
        }

        private async Task LoadBasicConversation(DocumentChange documentChange)
        {
            Android.Runtime.JavaList participants = (Android.Runtime.JavaList)documentChange.Document.Get(Constants.PARTICIPANTS);

            DocumentReference participantReference = null;
            foreach (DocumentReference documentReference in participants)
            {
                if (!documentReference.Equals(_referenceDocument))
                {
                    participantReference = documentReference;
                    break;
                }
            }

            DocumentSnapshot participant = await participantReference
                .Get()
                .AsAsync<DocumentSnapshot>();

            if (!TryUpdate(documentChange))
            {
                EntityPreview userPreview = new()
                {
                    Name = participant.GetString(Constants.USER_NAME),
                    ProfilePhoto = participant.GetString(Constants.USER_PHOTO),
                    PreviewText = documentChange.Document.GetString(Constants.LAST_MESSAGE),
                    LastChangeTime = documentChange.Document.GetString(Constants.MESSAGE_DATE).FormatStringToDate(),
                    EntityId = participant.Id,
                    ConversationId = documentChange.Document.Id,
                    IsGroup = false,
                };

                _chatPreviews.Add(userPreview);
            }
        }

        private bool TryUpdate(DocumentChange documentChange)
        {
            for (int i = 0; i < _chatPreviews.Count; i++)
            {
                if (_chatPreviews[i].ConversationId == documentChange.Document.Id)
                {
                    _chatPreviews[i].PreviewText = documentChange.Document.GetString(Constants.LAST_MESSAGE);
                    _chatPreviews[i].LastChangeTime = documentChange.Document.GetString(Constants.MESSAGE_DATE).FormatStringToDate();
                    return true;
                }
            }

            return false;
        }

        public void OpenChat(EntityPreview entityPreview)
        {
            Intent intent = new(Context, typeof(ChatActivity));
            intent.PutExtra(Constants.USER_ID, JsonConvert.SerializeObject(entityPreview));
            StartActivity(intent);
        }
    }
}