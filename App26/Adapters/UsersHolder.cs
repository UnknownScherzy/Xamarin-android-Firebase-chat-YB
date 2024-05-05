using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace App26.Adapters
{
    /// <summary>
    /// Used to initialize UserPreview layout in RecyclerView
    /// </summary>
    public class UsersHolder : RecyclerView.ViewHolder
    {
        public CheckBox Selector { get; private set; }

        private View _view;

        public UsersHolder(View itemView) : base(itemView)
        {
            _view = itemView;
            Selector = _view.FindViewById<CheckBox>(Resource.Id.previewCheckBox);
        }

        public void SetData(EntityPreview entityPreview)
        {
            _view.FindViewById<TextView>(Resource.Id.userName).Text = entityPreview.Name;
            _view.FindViewById<TextView>(Resource.Id.messagePreview).Text = entityPreview.PreviewText;
            _view.FindViewById<ImageView>(Resource.Id.userProfilePhoto).SetImageBitmap(entityPreview.ProfilePhoto.DecodeImage());
        }
    }
}