using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using App26.Entity;
using System.Collections.Generic;

namespace App26.Adapters
{
    public class ChatAdapter : RecyclerView.Adapter
    {
        private List<ChatMessage> _messages;

        private readonly string _senderId;
        private readonly Context _context;

        public override int ItemCount => _messages.Count;

        public const int SENDER = 1;
        public const int RECEIVER = 2;

        public ChatAdapter(List<ChatMessage> messages, Context context, string senderId)
        {
            _messages = messages;
            _context = context;
            _senderId = senderId;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((MessageHolder)holder).SetData(_messages[position], GetItemViewType(position));
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            int resource = viewType == SENDER ? Resource.Layout.message : Resource.Layout.message_received;
            return new MessageHolder(LayoutInflater.FromContext(_context).Inflate(resource, parent, false));
        }

        public override int GetItemViewType(int position)
        {
            return _messages[position].SenderId == _senderId ? SENDER : RECEIVER;
        }
    }
}