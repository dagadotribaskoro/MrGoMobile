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
using MrGo.Models;
using Android.Graphics;
using Java.Net;
using System.IO;
using Android.Graphics.Drawables;
using Android.Content.Res;

namespace MrGo.Entity
{
    public class Util
    {
        public static List<FriendViewModel> GenerateFriends()
        {
            return new List<FriendViewModel>
                        {
                            new FriendViewModel
                                {
                                    Title = "Japanese Macaque",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/japanese-macaque_589_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Golden Lion Tamarin",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/golden-lion-tamarin_552_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Mandrill",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/mandrill_622_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Gelada Monkeys",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/gelada-baboons_536_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Howler Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/004/overrides/black-howler-monkey_467_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Spider Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/007/overrides/spider-monkey_719_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Rhesus Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/rhesus-monkey_684_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Vervet Monkeys",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/064/cache/vervet-monkey_6458_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Olive Baboons",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/olive-baboon_649_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Squirrel Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/064/cache/squirrel-monkey_6457_100x75.jpg"
                                },
                            new FriendViewModel
                                {
                                    Title = "Proboscis Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/064/cache/proboscis-monkey_6456_100x75.jpg"
                                }
                        };
        }

        public static List<RestaurantViewModel> GenerateRestaurants()
        {
            return new List<RestaurantViewModel>
                        {
                            new RestaurantViewModel
                                {
                                    Title = "Japanese Macaque",
                                    Image = "http://mrgoapps.hol.es/restoimage/000007.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Golden Lion Tamarin",
                                    Image = "http://mrgoapps.hol.es/restoimage/000006.JPG"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Mandrill",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/mandrill_622_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Gelada Monkeys",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/gelada-baboons_536_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Howler Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/004/overrides/black-howler-monkey_467_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Spider Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/007/overrides/spider-monkey_719_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Rhesus Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/rhesus-monkey_684_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Vervet Monkeys",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/064/cache/vervet-monkey_6458_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Olive Baboons",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/olive-baboon_649_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Squirrel Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/064/cache/squirrel-monkey_6457_100x75.jpg"
                                },
                            new RestaurantViewModel
                                {
                                    Title = "Proboscis Monkey",
                                    Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/064/cache/proboscis-monkey_6456_100x75.jpg"
                                }
                        };
        }

        //public static List<RestoViewModel> GenerateResto()
        //{
        //    return new List<RestoViewModel>
        //                {
        //                    new RestoViewModel
        //                        {
        //                            Title = "Japanese Macaque",
        //                            Image = "http://mrgoapps.hol.es/restoimage/000007.jpg"
        //                        },
        //                    new RestoViewModel
        //                        {
        //                            Title = "Golden Lion Tamarin",
        //                            Image = "http://mrgoapps.hol.es/restoimage/000006.JPG"
        //                        },
        //                    new RestoViewModel
        //                        {
        //                            Title = "Mandrill",
        //                            Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/006/overrides/mandrill_622_100x75.jpg"
        //                        },
        //                    new RestoViewModel
        //                        {
        //                            Title = "Gelada Monkeys",
        //                            Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/gelada-baboons_536_100x75.jpg"
        //                        },
        //                    new RestoViewModel
        //                        {
        //                            Title = "Howler Monkey",
        //                            Image = "http://images.nationalgeographic.com/wpf/media-live/photos/000/004/overrides/black-howler-monkey_467_100x75.jpg"
        //                        },
        //                };
        //}
        //public static Bitmap getBitmapFromURL(Resources r)
        //{
        //    try
        //    {
        //        string imageUrl = "http://images.nationalgeographic.com/wpf/media-live/photos/000/005/overrides/japanese-macaque_589_100x75.jpg";
        //        URL url = new URL(imageUrl);
        //        Bitmap bitmap = BitmapFactory.DecodeStream(url.OpenConnection().InputStream);
        //        BitmapDrawable background = new BitmapDrawable( r, bitmap);
        //        rightv.setBackgroundDrawable(background);
        //    }
        //    catch (IOException e)
        //    {
        //     //   e.printStackTrace();
        //        return null;
        //    }
        //}
    }
}
