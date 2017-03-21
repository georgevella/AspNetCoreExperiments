using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Net.Http;
using Android.Widget;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Glyde.Bookmarks.LinkManagement.Resources;
using Newtonsoft.Json;

namespace Glyde.Bookmarks.AndroidApp
{
    [Activity(Label = "Bookmarks", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(toolbar);
            ActionBar.Title = GetString(Resource.String.ApplicationName);

            PopulateBookmarks();
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);            
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.top_menus, menu);

            var searchItem = menu.FindItem(Resource.Id.action_search);

            //var searchView = searchItem.ActionProvider.JavaCast<Android.Widget.SearchView>();

            //searchView.QueryTextSubmit += (sender, args) =>
            //{
            //    Toast.MakeText(this, "You searched: " + args.Query, ToastLength.Short).Show();
            //};            

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_refresh:
                    PopulateBookmarks();
                    break;
                default:
                    Toast.MakeText(this, "Action selected: " + item.TitleFormatted,
                        ToastLength.Short).Show();
                    break;
            }
           
            return base.OnOptionsItemSelected(item);
        }

        private async Task PopulateBookmarks()
        {
            var ls = FindViewById<ListView>(Resource.Id.bookmarkList);

            var result = await FetchAsync(@"http://169.254.80.80:8181/api/v1/bookmarks");
            ls.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1,
                result.Select(x => x.Title).ToArray());
        }

        private async Task<IEnumerable<Bookmark>> FetchAsync(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                    using (var streamReader = new StreamReader(stream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    var items = JsonSerializer.Create().Deserialize<IEnumerable<Bookmark>>(reader);
                    
                    // Return the JSON document:
                    return items;
                }
            }
        }
    }
}

