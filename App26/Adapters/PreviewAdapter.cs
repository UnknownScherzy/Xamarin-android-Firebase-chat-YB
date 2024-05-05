using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using App26.Adapters;
using App26.Entity;
using System.Collections.Generic;

namespace App26
{
    public class PreviewAdapter : RecyclerView.Adapter
    {
        private IChat _chatListener;
        private ISelectable _selectable;

        private List<EntityPreview> _entityPreviews;
        private Context _context;
        private int _selectedCount;

        public bool IsSelectionMode { get; private set; }

        public override int ItemCount => _entityPreviews.Count;

        public PreviewAdapter(List<EntityPreview> entityPreviews, Context context, IChat chatListener, ISelectable selectable = null)
        {
            _selectedCount = 0;
            _entityPreviews = entityPreviews;
            _context = context;
            _chatListener = chatListener;
            _selectable = selectable;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            UsersHolder usersHolder = (UsersHolder)holder;
            usersHolder.SetData(_entityPreviews[position]);

            usersHolder.Selector.Visibility = IsSelectionMode ? ViewStates.Visible : ViewStates.Gone;

            if (!usersHolder.ItemView.HasOnClickListeners)
            {
                usersHolder.ItemView.Click += delegate
                {
                    if (IsSelectionMode)
                    {
                        _selectable.Select(_entityPreviews[position]);
                        usersHolder.Selector.Checked = !usersHolder.Selector.Checked;
                        _selectedCount += usersHolder.Selector.Checked ? 1 : -1;

                        if (_selectedCount <= 0)
                        {
                            _selectable.OnSelectionFinished();
                            IsSelectionMode = false;
                            NotifyDataSetChanged();
                            return;
                        }
                    }
                    else
                    {
                        _chatListener.OpenChat(_entityPreviews[position]);
                    }
                };
            }

            if (_selectable != null && !usersHolder.ItemView.HasOnLongClickListeners)
            {
                usersHolder.ItemView.LongClick += delegate
                {
                    IsSelectionMode = !IsSelectionMode;

                    if (IsSelectionMode)
                    {
                        _selectable.OnSelectionStarted();
                        usersHolder.Selector.Checked = !usersHolder.Selector.Checked;
                        _selectable.Select(_entityPreviews[position]);
                        _selectedCount++;
                    }
                    else
                    {
                        _selectable.OnSelectionFinished();
                        _selectedCount = 0;
                    }

                    NotifyDataSetChanged();
                };
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new UsersHolder(LayoutInflater.FromContext(_context).Inflate(Resource.Layout.user_preview, parent, false));
        }
    }
}