using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using App26.Adapters;
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
    [Activity(Label = "ChatActivity")]
    public class ChatActivity : AppCompatActivity, Firebase.Firestore.IEventListener
    {
        private List<ChatMessage> _messages;
        private EntityPreview _entityPreviewData;

        private RecyclerView _recyclerView;
        private ImageView _groupSettings;

        private ChatAdapter _chatAdapter;
        private EditText _input;

        private string _myId;

        private DocumentSnapshot _conversation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_chat);

            _entityPreviewData = JsonConvert.DeserializeObject<EntityPreview>(Intent.GetStringExtra(Constants.USER_ID));
            _myId = LocalDatabase.GetString(Constants.USER_ID);

            Init();

            if (!_entityPreviewData.IsGroup)
            {
                _entityPreviewData.ConversationId = CreateConversationId();
            }
            else
            {
                _groupSettings.Visibility = ViewStates.Visible;
            }

            MessageListener();
        }

        private string CreateConversationId()
        {
            if (_entityPreviewData.EntityId.GetHashCode() < _myId.GetHashCode())
            {
                return _entityPreviewData.EntityId + "_" + _myId;
            }

            return _myId + "_" + _entityPreviewData.EntityId;
        }

        private void Init()
        {
            FindViewById<TextView>(Resource.Id.chatName).Text = _entityPreviewData.Name;

            _groupSettings = FindViewById<ImageView>(Resource.Id.chatSettingsButton);
            _messages = new List<ChatMessage>();

            _input = FindViewById<EditText>(Resource.Id.chatInputMessage);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.charRecyclerView);
            _chatAdapter = new(_messages, this, _myId);
            _recyclerView.SetAdapter(_chatAdapter);


            FindViewById<FrameLayout>(Resource.Id.sendLayout).Click += delegate
            {
                SendMessage();
            };

            FindViewById<ImageView>(Resource.Id.imageBack).Click += delegate
            {
                StartActivity(new Intent(this, typeof(DashboardActivity)));
            };
        }

        private void MessageListener()
        {
            FirebaseDataHelper.Database
                             .Collection(Constants.CONVERSATION_TABLE_ID)
                            .Document(_entityPreviewData.ConversationId)
                            .Collection(Constants.MESSAGES)
                            .AddSnapshotListener(this);
        }

        private async void SendMessage()
        {
            string inputMessage = _input.Text.Trim();

            if (inputMessage == string.Empty)
            {
                return;
            }

            if (_conversation == null || !_conversation.Exists())
            {
                await ChceckConversation(inputMessage);
            }

            _input.Text = string.Empty;
            await SendMessageRoutine(inputMessage);
        }

        private async Task ChceckConversation(string message)
        {
            _conversation = await FirebaseDataHelper.Database
                .Collection(Constants.CONVERSATION_TABLE_ID)
                .Document(_entityPreviewData.ConversationId)
                .Get()
                .AsAsync<DocumentSnapshot>();

            if (_conversation == null || !_conversation.Exists())
            {
                await FirebaseDataHelper.Database
                .Collection(Constants.CONVERSATION_TABLE_ID)
                .Document(_entityPreviewData.ConversationId)
                .Set(CreateConversation(message))
                .AsAsync<DocumentReference>();
            }
        }

        private async Task SendMessageRoutine(string inputMessage)
        {
            await FirebaseDataHelper.Database
                .Collection(Constants.CONVERSATION_TABLE_ID)
                .Document(_entityPreviewData.ConversationId)
                .Collection(Constants.MESSAGES)
                .Add(CreateMessage(inputMessage))
                .AsAsync();

            await FirebaseDataHelper.Database
                .Collection(Constants.CONVERSATION_TABLE_ID)
                .Document(_entityPreviewData.ConversationId)
                .Update(Constants.LAST_MESSAGE, inputMessage,
                        Constants.MESSAGE_DATE, DateTime.Now.FormatDate())
                .AsAsync();
        }

        private HashMap CreateConversation(string message)
        {
            HashMap conversation = new();
            conversation.Put(Constants.PARTICIPANTS, CreateParticipants());
            conversation.Put(Constants.LAST_MESSAGE, message);
            conversation.Put(Constants.IS_GROUP, false);
            conversation.Put(Constants.MESSAGE_DATE, DateTime.Now.FormatDate());

            return conversation;
        }

        private ArrayList CreateParticipants()
        {
            ArrayList participants = new();
            participants.Add(GetParticipantReferenceById(LocalDatabase.GetString(Constants.USER_ID)));
            participants.Add(GetParticipantReferenceById(_entityPreviewData.EntityId));
            return participants;
        }

        private DocumentReference GetParticipantReferenceById(string id)
        {
            return FirebaseDataHelper.Database
                .Collection(Constants.USERS_TABLE_ID)
                .Document(id);
        }

        private HashMap CreateMessage(string message)
        {
            HashMap newMessage = new();
            newMessage.Put(Constants.MESSAGE_TEXT, message);
            newMessage.Put(Constants.MESSAGE_DATE, DateTime.Now.FormatDate());
            newMessage.Put(Constants.SENDER_ID, _myId);
            newMessage.Put(Constants.USER_PHOTO, LocalDatabase.GetString(Constants.USER_PHOTO));

            return newMessage;
        }

        //OnConversationChanged..
        public void OnEvent(Java.Lang.Object obj, FirebaseFirestoreException error)
        {
            if (error != null || obj == null)
            {
                return;
            }

            if (obj is QuerySnapshot querySnapshot)
            {
                foreach (DocumentChange documentChange in querySnapshot.DocumentChanges)
                {
                    if (documentChange.GetType() != DocumentChange.Type.Added)
                    {
                        continue;
                    }

                    ChatMessage message = new()
                    {
                        Text = documentChange.Document.GetString(Constants.MESSAGE_TEXT),
                        SenderId = documentChange.Document.GetString(Constants.SENDER_ID),
                        PhotoHash = documentChange.Document.GetString(Constants.USER_PHOTO),
                        Date = documentChange.Document.GetString(Constants.MESSAGE_DATE).FormatStringToDate(),
                    };

                    _messages.Add(message);
                }

                _messages.Sort((x, y) => x.Date.CompareTo(y.Date));
                _chatAdapter.NotifyDataSetChanged();
                _recyclerView.SmoothScrollToPosition(_messages.Count);
            }
        }
    }
}