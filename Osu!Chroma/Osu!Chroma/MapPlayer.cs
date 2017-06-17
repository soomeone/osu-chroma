using Corale.Colore.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Corale.Colore.Core.Color;
using System.Threading;

namespace Osu_Chroma
{
    class MapPlayer
    {
        String mapfile = null;
        Color[] colors = null;
        List<TimingPoint> timingpoints = new List<TimingPoint>();

        public MapPlayer(String map)
        {
            mapfile = map;

            // Parse map
            List<Color> c = new List<Color>();
            String section = "";
            int lastcombo = 0;
            foreach (String line in File.ReadAllLines(map))
            {
                Console.WriteLine(line);
                if (line.StartsWith("["))
                    section = line; // Update section
                else if (line.StartsWith("Combo") && section == "[Colours]")
                {
                    String[] args = line.Split(':');
                    args[1] = args[1].Substring(1); // Remove the space after :

                    String[] rgb = args[1].Split(',');
                    c.Add(new Color(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]))); // Add new color


                    Console.WriteLine("Added new color");
                }
                else if (section == "[HitObjects]")
                {
                    String[] args = line.Split(',');
                    if (args.Length > 3)
                    {
                        int time = int.Parse(args[2]);
                        int combo = int.Parse(args[3]);

                        // Add only on new combo
                        if (combo != lastcombo)
                        {
                            timingpoints.Add(new TimingPoint(time, combo));
                            Console.WriteLine("Added a timingpoint at " + time);
                        }
                        lastcombo = combo;

                    }
                }
            }

            // Add default color if list is empty
            if (c.Count <= 0)
                c.Add(new Color(255, 136, 149));

            // Convert list into array
            colors = c.ToArray();
        }

        long starttime = 0; // Time when map started, so the current time can be calculated
        Boolean started = false;
        Thread timingthread;

        public async void Start()
        {
            Thread.Sleep(1000);
            timingthread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;


                int currentcombo = 0;
                int time = 0;
                while (started)
                {
                    if (timingpoints.Count >= 1)
                    {
                        Thread.Sleep(1); // Check every millisecond
                        TimingPoint nextpoint = timingpoints.ElementAt(0);

                        Console.WriteLine("Current:" + time);
                        Console.WriteLine("Next:" + nextpoint.Time);

                        if (nextpoint.Time < time)
                        {
                            Console.WriteLine("Timepoint reched");

                            timingpoints.Remove(nextpoint); // Remove timing point
                            if (colors.Length > nextpoint.Color)
                                SetColor(colors[nextpoint.Color]); // Set the current color
                            currentcombo++;
                            if (currentcombo >= colors.Length)
                                currentcombo = 0;
                        }
                        time++;
                    }
                }
            });

            starttime = DateTime.Now.Millisecond; // Set start time of song
            started = true;

            timingthread.Start();
        }

        public long getMillis()
        {
            // Return time of current song
            return DateTime.Now.Millisecond - starttime;
        }

        public void Stop()
        {
            started = false;
            timingthread.Abort();
        }

        public void SetColor(Color c)
        {
            Chroma.Instance.SetAll(c);
        }
    }
}
