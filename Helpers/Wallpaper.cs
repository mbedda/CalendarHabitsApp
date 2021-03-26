using CalendarHabitsApp.Models;
using Microsoft.Win32;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using IOPath = System.IO.Path;

namespace CalendarHabitsApp.Helpers
{
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }

        public static Color color1Light = Color.FromRgb(31, 31, 31);
        public static Color color2Light = Color.FromRgb(236, 235, 220);
        public static Color color3Light = Color.FromRgb(236, 235, 220);
        public static String baseImageLight = "base.png";
        public static String highlightLight = "highlight.png";
        public static String crossoutLight = "x-dark.png";
        public static String crossoutDimmedLight = "x-light.png";

        public static Color color1Dark = Color.FromRgb(236, 235, 220);
        public static Color color2Dark = Color.FromRgb(236, 235, 220);
        public static Color color3Dark = Color.FromRgb(77, 74, 74);
        public static String baseImageFileDark = "base-dark.png";
        public static String highlightFileDark = "highlight-dark.png";
        public static String crossoutFileDark = "x-dark-dark.png";
        public static String crossoutDimmedFileDark = "x-light-dark.png";

        public static void CreateCalendar(bool darkMode, DateTime currentDate, CalendarCell currentDateCell, List<MonthDay> selectedMonthDays, List<DateTime> habitDays)
        {
            CreateImage(darkMode, currentDate, currentDateCell, selectedMonthDays, habitDays);
        }

        public static void Set(string path)
        {
            MainWindow.log.Info("Fetching desktop registry key");
            Style style = Wallpaper.Style.Centered;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                MainWindow.log.Info("Setting WallpaperStyle key value");
                key.SetValue(@"WallpaperStyle", 1.ToString());
                MainWindow.log.Info("Setting TileWallpaper key value");
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            MainWindow.log.Info("Saving wallpaper path to registry - " + path);
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            MainWindow.log.Info("Saving wallpaper complete");
        }

        public static void CreateImage(bool darkMode, DateTime currentDate, CalendarCell currentDateCell, List<MonthDay> selectedMonthDays, List<DateTime> habitDays)
        {
            MainWindow.log.Info("Drawing calendar bitmap");
            Color color1, color2, color3;
            String baseImageFile, highlightFile, crossoutFile, crossoutDimmedFile;

            if (darkMode)
            {
                color1 = color1Dark;
                color2 = color2Dark;
                color3 = color3Dark;
                baseImageFile = baseImageFileDark;
                highlightFile = highlightFileDark;
                crossoutFile = crossoutFileDark;
                crossoutDimmedFile = crossoutDimmedFileDark;
            }
            else
            {
                color1 = color1Light;
                color2 = color2Light;
                color3 = color3Light;
                baseImageFile = baseImageLight;
                highlightFile = highlightLight;
                crossoutFile = crossoutLight;
                crossoutDimmedFile = crossoutDimmedLight;
            }

            //Init
            //
            CalendarVariables calVars = new CalendarVariables();

            int textw = 70;
            int texth = 45;

            FontCollection collection = new FontCollection();
            FontFamily family = collection.Install(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "Fonts", "OldStandard-Bold.ttf"));


            Font numbersFont = family.CreateFont(60, FontStyle.Italic);
            RendererOptions numbersFontOptions = new RendererOptions(numbersFont, dpi: 72)
            {
                ApplyKerning = true
            };

            using (Image<Rgba32> baseImage = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", baseImageFile))) // load up source images
            using (Image<Rgba32> highlightImage = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", highlightFile)))
            using (Image<Rgba32> crossoutImage = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", crossoutFile)))
            using (Image<Rgba32> crossoutDisabledImage = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", crossoutDimmedFile)))
            using (Image<Rgba32> outputImage = new Image<Rgba32>(baseImage.Width, baseImage.Height)) // create output image of the correct dimensions
            {
                #region Highlight current day on calendar
                //Highlight current day on calendar
                //
                Point highlightPosition = new Point(
                    calVars.canvasStartX + (currentDateCell.X - 1) * calVars.calendarCellW
                    + (currentDateCell.X * calVars.calendarCellBorder)
                    + (calVars.calendarCellW / 2 - calVars.currentDayHighlightImageW / 2),
                     calVars.canvasStartY - 2 + ((currentDateCell.Y - 1) * calVars.calendarCellH)
                    + (currentDateCell.Y * calVars.calendarCellBorder)
                    + (calVars.calendarCellH / 2 - calVars.currentDayHighlightImageH / 2));
                //Y calc -> (CanvasStart Y px + CurrentDayCell row number * cellheight) + (CurrentDayCell row number * border size) + (cellheight / 2 - imageheight / 2)

                outputImage.Mutate(o => o
                    .DrawImage(baseImage, new Point(0, 0), 1f)
                    .DrawImage(highlightImage, highlightPosition, 1f)
                );
                #endregion

                #region Draw Calendar numbers
                //Draw Calendar numbers
                //
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        float posx = calVars.canvasStartX + ((j + 1) - 1) * calVars.calendarCellW + ((j + 1) * calVars.calendarCellBorder) + (calVars.calendarCellW / 2 - textw / 2);
                        float posy = calVars.canvasStartY - 20 + (((i + 1) - 1) * calVars.calendarCellH) + ((i + 1) * calVars.calendarCellBorder) + (calVars.calendarCellH / 2 - texth / 2);

                        Color textcolor = color1;

                        if (!selectedMonthDays[(i * 7) + j].FromCurrentMonth)
                        {
                            textcolor = color3;
                        }

                        outputImage.Mutate(x => x.DrawText(selectedMonthDays[(i * 7) + j].Date.Day.ToString("D2"), numbersFont, textcolor, new PointF(posx, posy)));

                        //Crossout completed habit days
                        //
                        var habitDay = habitDays.FindIndex(s => s.ToString("MM/dd/yyyy") == selectedMonthDays[(i * 7) + j].Date.ToString("MM/dd/yyyy"));
                        if (habitDay != -1)
                        {
                            int posx2 = calVars.canvasStartX - 10 + ((j + 1) - 1) * calVars.calendarCellW + ((j + 1) * calVars.calendarCellBorder) + (calVars.calendarCellW / 2 - textw / 2);
                            int posy2 = calVars.canvasStartY - 22 + ((i + 1) - 1) * calVars.calendarCellH + ((i + 1) * calVars.calendarCellBorder) + (calVars.calendarCellH / 2 - texth / 2);

                            if (selectedMonthDays[(i * 7) + j].FromCurrentMonth)
                            {
                                outputImage.Mutate(x => x.DrawImage(crossoutImage, new Point(posx2, posy2), 1f));
                            }
                            else
                            {
                                outputImage.Mutate(x => x.DrawImage(crossoutDisabledImage, new Point(posx2, posy2), 1f));
                            }
                        }
                    }
                }
                #endregion

                #region Draw month name text
                //Draw month name text
                //
                PathBuilder pathBuilder = new PathBuilder();
                pathBuilder.AddLine(new PointF(1930, 1008), new PointF(1930, 73));

                IPath path = pathBuilder.Build();

                var textGraphicsOptions = new TextGraphicsOptions() // draw the text along the path wrapping at the end of the line
                {
                    TextOptions = {
                        WrapTextWidth = path.Length,
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                };

                Font monthFontBase = family.CreateFont(242, FontStyle.Italic);
                RendererOptions monthFontOptions = new RendererOptions(monthFontBase, dpi: 72)
                {
                    ApplyKerning = true
                };

                string MonthText = Common.GetMonthNameFromNumber(currentDate.Month).ToUpper();

                // measure the text size
                FontRectangle size = TextMeasurer.Measure(MonthText, new RendererOptions(monthFontBase));
                int w = 934;
                int h = 278;
                float scalingFactor = Math.Min(w / size.Width, h / size.Height);
                Font monthFont = new Font(monthFontBase, scalingFactor * monthFontBase.Size);

                var glyphs = TextBuilder.GenerateGlyphs(MonthText, path, new RendererOptions(monthFont, textGraphicsOptions.TextOptions.DpiX, textGraphicsOptions.TextOptions.DpiY)
                {
                    HorizontalAlignment = textGraphicsOptions.TextOptions.HorizontalAlignment,
                    TabWidth = textGraphicsOptions.TextOptions.TabWidth,
                    VerticalAlignment = textGraphicsOptions.TextOptions.VerticalAlignment,
                    WrappingWidth = textGraphicsOptions.TextOptions.WrapTextWidth,
                    ApplyKerning = textGraphicsOptions.TextOptions.ApplyKerning
                });

                outputImage.Mutate(ctx => ctx
                    .Fill(color1, glyphs));
                #endregion


                #region Draw year text
                //Draw year text
                //
                PathBuilder pathBuilder2 = new PathBuilder();
                pathBuilder2.AddLine(new PointF(-10, 267), new PointF(-10, 829));

                IPath path2 = pathBuilder2.Build();

                var textGraphicsOptions2 = new TextGraphicsOptions() // draw the text along the path wrapping at the end of the line
                {
                    TextOptions = {
                        WrapTextWidth = path2.Length
                    }
                };

                Font yearFont = family.CreateFont(242);
                RendererOptions yearFontOptions = new RendererOptions(yearFont, dpi: 72)
                {
                    ApplyKerning = true
                };

                var glyphs2 = TextBuilder.GenerateGlyphs(currentDate.Year.ToString(), path2, new RendererOptions(yearFont, textGraphicsOptions2.TextOptions.DpiX, textGraphicsOptions2.TextOptions.DpiY)
                {
                    HorizontalAlignment = textGraphicsOptions2.TextOptions.HorizontalAlignment,
                    TabWidth = textGraphicsOptions2.TextOptions.TabWidth,
                    VerticalAlignment = textGraphicsOptions2.TextOptions.VerticalAlignment,
                    WrappingWidth = textGraphicsOptions2.TextOptions.WrapTextWidth,
                    ApplyKerning = textGraphicsOptions2.TextOptions.ApplyKerning
                });

                outputImage.Mutate(ctx => ctx
                    .Fill(color1, glyphs2));
                #endregion

                MainWindow.log.Info("Saving image as output.png");
                //Save output image
                //
                try
                {
                    outputImage.SaveAsPngAsync("output.png");
                    MainWindow.log.Info("Image saved successfully");
                }
                catch (Exception e)
                {
                    MainWindow.log.Error(e.InnerException.Message);
                }
            }
        }
    }
}
