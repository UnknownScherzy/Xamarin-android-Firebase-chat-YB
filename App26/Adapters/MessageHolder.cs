using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using App26.Entity;

namespace App26.Adapters
{
    public class MessageHolder : RecyclerView.ViewHolder
    {
        private View _view;

        public MessageHolder(View view) : base(view)
        {
            _view = view;
        }

        public void SetData(ChatMessage message, int viewType)
        {
            _view.FindViewById<TextView>(Resource.Id.messageText).Text = message.Text;
            _view.FindViewById<TextView>(Resource.Id.messageDate).Text = message.Date.ToString();

            if (viewType == ChatAdapter.RECEIVER)
            {
                _view.FindViewById<ImageView>(Resource.Id.profileImage).SetImageBitmap(message.PhotoHash.DecodeImage());
            }
        }
    }
}