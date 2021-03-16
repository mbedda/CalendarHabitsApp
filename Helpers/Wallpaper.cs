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

        public static void Set(Uri uri, Style style, int cellx, int celly, int year, DateTime currentDate, List<MonthDay> monthDays, List<DateTime> HabitDays)
        {
            //System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());
            Highlight(cellx, celly, year, currentDate, monthDays, HabitDays);
            //System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "output.png");
            //img.Save(tempPath, Imaging.ImageFormat.Bmp);
            //if(flag)
            //    tempPath = "D:/Work/Calendar.png";

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched)
            {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled)
            {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        public static void Highlight(int requiredcellx, int requiredcelly, int year, DateTime currentDate, List<MonthDay> monthDays, List<DateTime> HabitDays)
        {
            int startx = 344;
            int starty = 145;
            int cellw = 175;
            int cellh = 176;
            int cellborder = 1;

            int highlightwh = 140;

            int crossoutw = 109;
            int crossouth = 104;

            int textw = 89;
            int texth = 70;

            FontCollection collection = new FontCollection();
            FontFamily family = collection.Install(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "Fonts", "OldStandard-Bold.ttf"));
            Font numbersFont = family.CreateFont(75, FontStyle.Italic);
            RendererOptions numbersFontOptions = new RendererOptions(numbersFont, dpi: 72)
            {
                ApplyKerning = true
            };

            Font monthFont = family.CreateFont(242, FontStyle.Italic);
            RendererOptions monthFontOptions = new RendererOptions(numbersFont, dpi: 72)
            {
                ApplyKerning = true
            };

            System.IO.Directory.CreateDirectory("output");
            using (Image<Rgba32> img1 = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "base.png"))) // load up source images
            using (Image<Rgba32> img2 = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "highlight.png")))
            using (Image<Rgba32> img3 = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "x-dark.png")))
            using (Image<Rgba32> img4 = Image.Load<Rgba32>(IOPath.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "x-light.png")))
            using (Image<Rgba32> outputImage = new Image<Rgba32>(img1.Width, img1.Height)) // create output image of the correct dimensions
            {
                // take the 2 source images and draw them onto the image
                outputImage.Mutate(o => o
                    .DrawImage(img1, new Point(0, 0), 1f) // draw the first one top left
                    .DrawImage(img2, new Point(startx + (requiredcellx - 1) * cellw + (requiredcellx * cellborder)
                     + (cellw / 2 - highlightwh / 2), starty + (requiredcelly - 1) * cellh + (requiredcelly * cellborder) 
                     + (cellh / 2 - highlightwh / 2)), 1f) // draw the second next to it
                );

                //FontRectangle fontrect = TextMeasurer.Measure("01", numbersFontOptions);

                
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        float posx = startx + ((j + 1) - 1) * cellw + ((j + 1) * cellborder) + (cellw / 2 - textw / 2);
                        float posy = starty - 15 + ((i + 1) - 1) * cellh + ((i + 1) * cellborder) + (cellh / 2 - texth / 2);

                        Color textcolor = Color.FromRgb(31, 31, 31);

                        if (!monthDays[(i * 7) + j].FromCurrentMonth)
                        {
                            textcolor = Color.FromRgb(236, 235, 220);
                        }

                        outputImage.Mutate(x => x.DrawText(monthDays[(i * 7) + j].Date.Day.ToString("D2"), numbersFont, textcolor, new PointF(posx, posy)));

                        var habitDay = HabitDays.FindIndex(s => s.ToString("MM/dd/yyyy") == monthDays[(i * 7) + j].Date.ToString("MM/dd/yyyy"));
                        if (habitDay != -1)
                        {
                            int posx2 = startx - 10 + ((j + 1) - 1) * cellw + ((j + 1) * cellborder) + (cellw / 2 - textw / 2);
                            int posy2 = starty - 15 + ((i + 1) - 1) * cellh + ((i + 1) * cellborder) + (cellh / 2 - texth / 2);

                            if (monthDays[(i * 7) + j].FromCurrentMonth)
                            {
                                outputImage.Mutate(x => x.DrawImage(img3, new Point(posx2, posy2), 1f));
                            }
                            else
                            {
                                outputImage.Mutate(x => x.DrawImage(img4, new Point(posx2, posy2), 1f));
                            }
                        }
                    }
                }

               

                /////////////////
                ///


                PathBuilder pathBuilder = new PathBuilder();
                //pathBuilder.SetOrigin(new PointF(500, 0));
                pathBuilder.AddLine(new PointF(1930, 1008), new PointF(1930, 73));

                IPath path = pathBuilder.Build();

                var textGraphicsOptions = new TextGraphicsOptions() // draw the text along the path wrapping at the end of the line
                {
                    TextOptions = {
                        WrapTextWidth = path.Length
                    }
                };

                var glyphs = TextBuilder.GenerateGlyphs(currentDate.ToString("MMMM").ToUpper(), path, new RendererOptions(monthFont, textGraphicsOptions.TextOptions.DpiX, textGraphicsOptions.TextOptions.DpiY)
                {
                    HorizontalAlignment = textGraphicsOptions.TextOptions.HorizontalAlignment,
                    TabWidth = textGraphicsOptions.TextOptions.TabWidth,
                    VerticalAlignment = textGraphicsOptions.TextOptions.VerticalAlignment,
                    WrappingWidth = textGraphicsOptions.TextOptions.WrapTextWidth,
                    ApplyKerning = textGraphicsOptions.TextOptions.ApplyKerning
                });

                outputImage.Mutate(ctx => ctx
                    .Fill(Color.FromRgb(31, 31, 31), glyphs));

                /////////////////////


                /////////////////
                ///


                PathBuilder pathBuilder2 = new PathBuilder();
                //pathBuilder.SetOrigin(new PointF(500, 0));
                pathBuilder2.AddLine(new PointF(-10, 267), new PointF(-10, 829));

                IPath path2 = pathBuilder2.Build();

                var textGraphicsOptions2 = new TextGraphicsOptions() // draw the text along the path wrapping at the end of the line
                {
                    TextOptions = {
                        WrapTextWidth = path2.Length
                    }
                };

                var glyphs2 = TextBuilder.GenerateGlyphs(year.ToString(), path2, new RendererOptions(monthFont, textGraphicsOptions.TextOptions.DpiX, textGraphicsOptions.TextOptions.DpiY)
                {
                    HorizontalAlignment = textGraphicsOptions.TextOptions.HorizontalAlignment,
                    TabWidth = textGraphicsOptions.TextOptions.TabWidth,
                    VerticalAlignment = textGraphicsOptions.TextOptions.VerticalAlignment,
                    WrappingWidth = textGraphicsOptions.TextOptions.WrapTextWidth,
                    ApplyKerning = textGraphicsOptions.TextOptions.ApplyKerning
                });

                outputImage.Mutate(ctx => ctx
                    .Fill(Color.FromRgb(31, 31, 31), glyphs2));

                /////////////////////


                outputImage.Save("output.png");
            }
        }
    }
}
