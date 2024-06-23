using Android.OS;
using Android.Views;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using AndroidX.Fragment.App;

namespace App26
{
    public class MapFragment : Fragment, IOnMapReadyCallback
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Inflate the layout for this fragment
            View view = inflater.Inflate(Resource.Layout.map_fragment, container, false);

            // Obtain the SupportMapFragment and get notified when the map is ready to be used.
            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            return view;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            MarkerOptions markerOptions = new MarkerOptions();

            // TODO: Allow app to ask permission for user to get LATLANG. 
            LatLng myPos = new LatLng(32.164816, 34.826822);

            markerOptions.SetPosition(myPos);
            markerOptions.SetTitle("You are here!");
            googleMap.AddMarker(markerOptions);

            // Optional
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;

            // Update Camera    
            googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(myPos));
            googleMap.AnimateCamera(CameraUpdateFactory.ZoomTo(17));
        }
    }
}
