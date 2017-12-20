using System;
using System.Globalization;
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
        private static List<Models.calendar> SortedList = null;

        //Intial creation
        public static void Calendar(Grid grid, string x1, string y1, string x2, string y2, JArray data)
        {
            CrossLogger.Current.Info("Calendar", "Widget Processing Started");

            try
            {
                int.TryParse(x1, out int px);
                int.TryParse(y1, out int py);
                int.TryParse(x2, out int sx);
                int.TryParse(y2, out int sy);

                List<Models.Sitemap.Widget3> items = data.ToObject<List<Models.Sitemap.Widget3>>();

                foreach (Models.Sitemap.Widget3 item in items)
                {
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.label);

                    //Master Grid for Widget
                    Grid Widget_Grid = new Grid
                    {
                        RowDefinitions = new RowDefinitionCollection {
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
                    },
                        ColumnDefinitions = new ColumnDefinitionCollection {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    },
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        BackgroundColor = App.config.CellColor,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        VerticalOptions = LayoutOptions.StartAndExpand,
                    };
                    grid.Children.Add(Widget_Grid, px, px + sx, py, py + sy);

                    itemCalendar.Add(new Models.calItems
                    {
                        Label = item.label,
                        State = item.item.state,
                        Name = item.item.name,
                        grid = Widget_Grid,
                    });
                }

                lock (itemCalendar)
                {
                    Create_Calendar();
                }

                // Check if we've crossed midnight as first entry is now yesterday
                Device.StartTimer(TimeSpan.FromSeconds(60), () =>
                {
                    if ((SortedList != null) && (SortedList.Count > 0) && (DateTime.Today.Date != SortedList[0].Start.Date))
                    {
                        CrossLogger.Current.Debug("Calendar", "Update Calendar as we've crossed midnight");
                        lock (itemCalendar)
                        {
                            Create_Calendar();
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
        public static void Calendar_Update(Models.Events item)
        {
            CrossLogger.Current.Debug("Calendar", "Update" + item.ToString());

            //Loop and update with new data
            foreach (Models.calItems itemCal in itemCalendar)
            {
                if (itemCal.Name.Equals(item.topic))
                {
                    itemCal.State = item.value;
                }
            }

            lock (itemCalendar)
            {
                Create_Calendar();
            }
        }

        //Create / Update GUI
        private static void Create_Calendar()
        {
            try
            {
                #region ParseItems
                //We know there are x items. Divide by 4 and create and initialize an array for the items
                Models.calendar[] calEvents = new Models.calendar[itemCalendar.Count / 4];

                //Make sure we have something to process
                if (itemCalendar.Count == 0) return;

                itemCalendar[0].grid.Children.Clear();

                //Loop through the items
                foreach (Models.calItems item in itemCalendar)
                {
                    Dictionary<string, string> widgetKeyValuePairs = Helpers.SplitCommand(item.Label);

                    if (widgetKeyValuePairs.ContainsKey("item"))
                    {
                        //Event ID Number
                        int.TryParse(Regex.Replace(widgetKeyValuePairs["item"], @"\D", ""), out int id);
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
                            //DateTime tmp = DateTime.ParseExact(item.State, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                            DateTime tmp = DateTime.MinValue;
                            DateTime.TryParseExact(item.State, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp);

                            if (tmp == DateTime.MinValue)
                            {
                                DateTime.TryParseExact(item.State, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp);
                            }

                            if (tmp == DateTime.MinValue)
                            {
                                CrossLogger.Current.Error("Calendar", "Failed to DateTime convert: '" + item.State + "'");
                            }

                            calEvents[id].Start = tmp;
                        }
                        else if (widgetKeyValuePairs["item"].ToLower().Contains("end-time"))
                        {
                            CrossLogger.Current.Debug("Calendar", "Nr: " + id.ToString() + ", End:" + item.State);
                            //DateTime tmp = DateTime.ParseExact(item.State, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                            DateTime tmp = DateTime.MinValue;
                            DateTime.TryParseExact(item.State, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp);

                            if (tmp == DateTime.MinValue)
                            {
                                DateTime.TryParseExact(item.State, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.None, out tmp);
                            }

                            if (tmp == DateTime.MinValue)
                            {
                                CrossLogger.Current.Error("Calendar", "Failed to DateTime convert: '" + item.State + "'");
                            }
                            calEvents[id].End = tmp;
                        }
                    }
                }

                for (int i = 0; i < calEvents.Count(); i++)
                {
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
                            if (a != null)
                            {
                                guiEvents.Add(a);
                            }
                        }
                    }
                    else
                    {
                        Models.calendar a = GuiRecord(calEvents[i], 0);
                        if (a != null)
                        {
                            guiEvents.Add(a);
                        }
                    }
                }

                //Sort the list
                SortedList = guiEvents.OrderBy(x => x.Start).ToList();

                for (int i = 0; i < SortedList.Count; i++)
                {
                    //Remove duplicate Day/DayOfWeek
                    for (int j = i+1; j < SortedList.Count; j++)
                    {
                        if (SortedList[i].Day == SortedList[j].Day)
                        {
                            SortedList[j].Day = string.Empty;
                            SortedList[j].DayOfWeek = string.Empty;
                        }
                    }

                    CrossLogger.Current.Debug("Calendar", SortedList[i].Day + "," + SortedList[i].DayOfWeek + "," + SortedList[i].Title + "," + SortedList[i].Hours + "," + SortedList[i].Location);
                }

                //Add today if missing from list
                if (SortedList.Count > 0 && SortedList[0].Start.Date > DateTime.Today.Date)
                {
                    Models.calendar a = new Models.calendar
                    {
                        Start = DateTime.Today.Date,
                        Day = DateTime.Now.Day.ToString(),
                        DayOfWeek = DateTime.Now.DayOfWeek.ToString().Substring(0, 3),
                        Title = "Today"
                    };

                    SortedList.Insert(0, a);
                }
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
                    }),
                };
                lvCalendar.ItemTapped += OnItemTapped; //Prevent selection of items and background color

                itemCalendar[0].grid.Children.Add(lvCalendar, 0, 0); //, px, px + sx, py, py + sy);
                #endregion Render               

                lvCalendar.ItemAppearing += LvCalendar_ItemAppearing;
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Error("Calendar", "Crashed: " + ex.ToString());
            }
        }

        private static void LvCalendar_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            App.config.LastActivity = DateTime.Now;
        }

        private static void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            App.config.LastActivity = DateTime.Now; //Update lastActivity to reset Screensaver timer

            if (e == null) return;
            ((ListView)sender).SelectedItem = null;
        }

        private static Models.calendar GuiRecord(Models.calendar item, int j)
        {
            if ((item.Start != DateTime.MinValue) && (item.Start.AddDays(j) >= DateTime.Now.Date))
            {
                Models.calendar a = new Models.calendar
                {
                    Day = item.Start.AddDays(j).Day.ToString(),
                    DayOfWeek = item.Start.AddDays(j).DayOfWeek.ToString().Substring(0, 3),
                    Title = item.Title,
                    Location = item.Location,
                    Start = item.Start.AddDays(j)
                };

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
            {
                return null;
            }
        }
    }
}