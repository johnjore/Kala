using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Xamarin.Forms;
using Newtonsoft.Json.Linq;
using Plugin.Logger;

namespace Kala
{
    public partial class Widgets
    {
        //Keep track of calendar items. GUI needs a complete redraw on each update
        public static List<Models.calItems> itemCalendar = new List<Models.calItems>();
        public static List<Models.calendar> SortedList = null;

        //Intial creation
        public static void Calendar(Grid grid, string x1, string y1, string x2, string y2, JArray data)
        {
            CrossLogger.Current.Info("Calendar", "Widget Processing Started");
            //CrossLogger.Current.Debug("Calendar", data.ToString());

            try
            {
                //Size of Calendar widget
                int px = Convert.ToInt16(x1);
                int py = Convert.ToInt16(y1);
                int sx = Convert.ToInt16(x2);
                int sy = Convert.ToInt16(y2);

                //Items in Calendar widget
                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();

                //Loop through the items and save to custom list
                foreach (Models.Sitemap.Widget3 item in items)
                {
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);

                    Models.calItems a = new Models.calItems();
                    a.Label = item.label;
                    a.State = item.item.state;
                    a.Link = item.item.link;

                    a.grid = grid;
                    a.px = px;
                    a.py = py;
                    a.sx = sx;
                    a.sy = sy;

                    itemCalendar.Add(a);
                }

                lock (itemCalendar)
                {
                    Create_Calendar();
                }

                // Check if we've crossed midnight as first entry is now yesterday
                Device.StartTimer(TimeSpan.FromSeconds(60), () =>
                {
                    if (SortedList != null && SortedList.Count > 0)
                    {
                        if (DateTime.Today.Date != SortedList[0].Start.Date)
                        {
                            CrossLogger.Current.Debug("Calendar", "Update Calendar as we've crossed midnight");
                            lock (itemCalendar)
                            {
                                Create_Calendar();
                            }
                        }
                    }

                    return true;
                });
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Calendar", "Crashed: " + ex.ToString());
            }
        }

        //Update stored list
        public static void Calendar_Update(Models.Item item)
        {
            CrossLogger.Current.Debug("Calendar", "Update" + item.ToString());

            //Loop and update with new data
            foreach (Models.calItems itemCal in itemCalendar)
            {
                if (itemCal.Link.Equals(item.link))
                {
                    itemCal.State = item.state;
                }
            }

            lock (itemCalendar)
            {
                Create_Calendar();
            }
        }

        //Create / Update GUI
        public static void Create_Calendar()
        {
            try
            {
                #region ParseItems
                //We know there are x items. Divide by 4 and create and initialize an array for the items
                Models.calendar[] calEvents = new Models.calendar[itemCalendar.Count / 4];

                //Make sure we have something to process
                if (itemCalendar.Count == 0) return;

                //Get Grid and position
                Grid grid = itemCalendar[0].grid;
                int px = itemCalendar[0].px;
                int py = itemCalendar[0].py;
                int sx = itemCalendar[0].sx;
                int sy = itemCalendar[0].sy;

                //Loop through the items
                foreach (Models.calItems item in itemCalendar)
                {
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);

                    if (widgetKeyValuePairs.ContainsKey("item"))
                    {
                        //Event ID Number
                        int id = 0;
                        int.TryParse(Regex.Replace(widgetKeyValuePairs["item"], @"\D", ""), out id);
                        id--;

                        //Initialize
                        if (calEvents[id] == null) calEvents[id] = new Models.calendar();

                        //CrossLogger.Current.Debug("Calendar", "Item nr: " + id.ToString() + ", item: " + widgetKeyValuePairs["item"].ToUpper());
                        if (widgetKeyValuePairs["item"].ToLower().Contains("title"))
                        {
                            CrossLogger.Current.Debug("Calendar", "Nr: " + id.ToString() + ", Title:" + item.State);
                            calEvents[id].Title = item.State;
                        }
                        else if (widgetKeyValuePairs["item"].ToLower().Contains("location"))
                        {
                            CrossLogger.Current.Debug("Calendar", "Nr: " + id.ToString() + ", Location:" + item.State);
                            calEvents[id].Location = item.State;
                        }
                        else if (widgetKeyValuePairs["item"].ToLower().Contains("start-time"))
                        {
                            CrossLogger.Current.Debug("Calendar", "Nr: " + id.ToString() + ", Start:" + item.State);
                            DateTime tmp;
                            DateTime.TryParse(item.State, out tmp);
                            calEvents[id].Start = tmp;
                        }
                        else if (widgetKeyValuePairs["item"].ToLower().Contains("end-time"))
                        {
                            CrossLogger.Current.Debug("Calendar", "Nr: " + id.ToString() + ", End:" + item.State);
                            DateTime tmp;
                            DateTime.TryParse(item.State, out tmp);
                            calEvents[id].End = tmp;
                        }
                    }
                }

                for (int i = 0; i < calEvents.Count(); i++)
                {
                    //calEvents[i].Hours = calEvents[i].Start.Hour.ToString() + ":" + calEvents[i].Start.Minute.ToString() + "-" + calEvents[i].End.Hour.ToString() + ":"  + calEvents[i].End.Minute.ToString();
                    calEvents[i].Hours = calEvents[i].Start.ToString("HH:mm") + ":" + calEvents[i].End.ToString("HH:mm");
                    CrossLogger.Current.Debug("Calendar", "Title:" + calEvents[i].Title + ", Time:" + calEvents[i].Start + "-" + calEvents[i].End + ", Location:" + calEvents[i].Location + ", Hours:" + calEvents[i].Hours);
                }
                #endregion ParseItems

                #region GUI
                //calEvents contain the list of events. Parse it to create the GUI as elements may span multiple days
                List<Models.calendar> guiEvents = new List<Models.calendar>();

                for (int i = 0; i < calEvents.Length; i++)
                {
                    int days = (calEvents[i].End - calEvents[i].Start).Days;

                    if (days > 0)
                    {
                        for (int j = 0; j < days; j++)
                        {
                            Models.calendar a = GuiRecord(calEvents[i], j);
                            if (a != null) guiEvents.Add(a);
                        }
                    }
                    else
                    {
                        Models.calendar a = GuiRecord(calEvents[i], 0);
                        if (a != null) guiEvents.Add(a);
                    }
                }

                //Sort the list
                SortedList = guiEvents.OrderBy(x => x.Start).ToList();

                for (int i = 0; i < SortedList.Count; i++)
                {
                    //Remove duplicate Day/DayOfWeek
                    if (i != 0)
                    {
                        if (SortedList[i - 1].Day == SortedList[i].Day)
                        {
                            SortedList[i].Day = String.Empty;
                            SortedList[i].DayOfWeek = String.Empty;
                        }
                    }

                    CrossLogger.Current.Debug("Calendar", SortedList[i].Day + "," + SortedList[i].DayOfWeek + "," + SortedList[i].Title + "," + SortedList[i].Hours + "," + SortedList[i].Location);
                }

                //Add today if missing from list
                if (SortedList[0].Start != DateTime.Now)
                {
                    Models.calendar a = new Models.calendar();
                    DateTime dt = DateTime.Now;

                    a.Start = dt;
                    a.Day = dt.Day.ToString();
                    a.DayOfWeek = dt.DayOfWeek.ToString().Substring(0, 3);
                    a.Title = "Today";

                    SortedList.Insert(0, a);
                }

                CrossLogger.Current.Debug("Stop", "Stop");
                #endregion GUI

                #region Render
                ListView lvCalendar = new ListView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    SeparatorColor = App.config.BackGroundColor,
                    BackgroundColor = App.config.CellColor,
                    SeparatorVisibility = SeparatorVisibility.Default,
                    RowHeight = 100,
                    HasUnevenRows = true,
                    ItemsSource = SortedList,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        // Create views with bindings for displaying each property
                        Label titleLabel = new Label
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                            FontAttributes = FontAttributes.Bold,
                        };
                        titleLabel.SetBinding(Label.TextProperty, "Title");

                        Label hourLabel = new Label
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                        };
                        hourLabel.SetBinding(Label.TextProperty, "Hours");

                        Label locationLabel = new Label
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            TextColor = App.config.TextColor,
                            BackgroundColor = App.config.CellColor,
                        };
                        locationLabel.SetBinding(Label.TextProperty, "Location");

                        Label dayLabel = new Label
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            TextColor = App.config.ValueColor,
                            BackgroundColor = App.config.CellColor,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.End
                        };
                        dayLabel.SetBinding(Label.TextProperty, "Day");

                        Label whenLabel = new Label
                        {
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            TextColor = App.config.ValueColor,
                            BackgroundColor = App.config.CellColor,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.End
                        };
                        whenLabel.SetBinding(Label.TextProperty, "DayOfWeek");

                        // Return an assembled ViewCell
                        return new ViewCell
                        {
                            View = new StackLayout
                            {
                                Padding = new Thickness(1, 1, 1, 1),
                                Orientation = StackOrientation.Horizontal,
                                Children =
                                {
                                    new StackLayout
                                    {
                                        Padding = new Thickness(10, 1, 10, 1),
                                        VerticalOptions = LayoutOptions.Start,
                                        Spacing = 0,
                                        WidthRequest = 50,
                                        MinimumWidthRequest = 50,
                                        Children =
                                        {
                                            dayLabel,
                                            whenLabel
                                        }
                                    },
                                    new StackLayout
                                    {
                                        Padding = new Thickness(10, 1, 0, 1),
                                        VerticalOptions = LayoutOptions.Start,
                                        Spacing = 5,
                                        Children =
                                        {
                                            titleLabel,
                                            hourLabel,
                                            locationLabel
                                        }
                                    }
                                }
                            }
                        };
                    })
                };

                grid.Children.Add(lvCalendar, px, px + sx, py, py + sy);
                #endregion Render               
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Calendar", "Crashed: " + ex.ToString());
            }
        }

        private static Models.calendar GuiRecord(Models.calendar item, int j)
        {
            if (item.Start != DateTime.MinValue)
            {
                Models.calendar a = new Models.calendar();

                a.Day = item.Start.AddDays(j).Day.ToString();
                a.DayOfWeek = item.Start.AddDays(j).DayOfWeek.ToString().Substring(0, 3);
                a.Title = item.Title;
                a.Location = item.Location;
                a.Start = item.Start.AddDays(j);

                if (item.Start.TimeOfDay == TimeSpan.Zero)
                {
                    a.Hours = "";
                }
                else
                {
                    a.Hours = item.Start.ToString("HH:mm") + " - " + item.End.ToString("HH:mm");
                }

                CrossLogger.Current.Debug("Calendar", a.Day + "," + a.DayOfWeek + "," + a.Title + "," + a.Hours + "," + a.Location);
                return a;
            }
            else
                return null;
        }
    }
}