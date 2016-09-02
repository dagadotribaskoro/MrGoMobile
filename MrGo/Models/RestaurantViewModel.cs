using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MrGo.Entity;

namespace MrGo.Models
{
    public class RestaurantViewModel
    {
        private string m_Image;
        public string Image
        {
            get { return this.m_Image; }
            set { this.m_Image = value; }
        }
        private long m_Id;
        /// <summary>
        /// Gets or sets the unique ID for the menu
        /// </summary>
        public long Id
        {
            get { return this.m_Id; }
            set { this.m_Id = value; }
        }
        private string m_Title = string.Empty;
        /// <summary>
        /// Gets or sets the name of the menu
        /// </summary>
        public string Title
        {
            get { return this.m_Title; }
            set { this.m_Title = value; }
        }
        public void Init(int id, string title, string image)
        {
            this.Id = id;
            this.Title = title;
            this.Image = image;
            this.Items = Util.GenerateRestaurants();
            this.Items.RemoveRange(0, this.Items.Count - 2);
        }
        private List<RestaurantViewModel> m_Items;
        public List<RestaurantViewModel> Items
        {
            get { return this.m_Items; }
            set { this.m_Items = value; }
        }
    }
}