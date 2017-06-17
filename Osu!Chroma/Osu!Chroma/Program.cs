using Corale.Colore.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ColoreColor = Corale.Colore.Core.Color;

namespace Osu_Chroma
{
    class Program
    {
        // One player for the map playing
        static MapPlayer Player;
        static String OsuFolder = null;

        static void Main(string[] args)
        {
            /*
             * Partial code from 
             * https://github.com/IAreKyleW00t/osu-np/blob/master/Program.cs
             */


            String title = null, song = null, currentsong = null;
            while (true)
            {
                /* Wait 1 second */
                Thread.Sleep(10);

                /* Check for osu! process */
                Process[] ps = Process.GetProcessesByName("osu!");
                if (ps.Length > 0)
                {
                    /* Save osu! process */
                    Process osu = ps[0];

                    /* Parse osu! window Title */
                    title = osu.MainWindowTitle;


                    if (OsuFolder == null)
                    {
                        // Get osu! folder
                        OsuFolder = osu.MainModule.FileName;
                        OsuFolder = OsuFolder.Substring(0, OsuFolder.Length - "osu!.exe".Length); // Trim

                        // Create the index on first time
                        CreateIndex();
                    }

                    /* Check if we're playing a song */
                    if (title.IndexOf('-') < 0)
                    {
                        /* Set default song */
                        song = null;
                        
                        continue;
                    }

                    /* Parse song from title */
                    song = title.Substring(title.IndexOf('-') + 2);

                    // Check if song changed
                    if (song != currentsong)
                    {

                        // Check if it's the selection or a real song
                        if (song != null)
                        {
                            /* Outout song to Console and song_file */
                            Console.WriteLine(song);

                            // Start new song
                            String map = getMapFile(song);
                            if (map != null)
                            {
                                Player = new MapPlayer(map);
                                Player.Start();
                            }
                        }
                        else
                        {
                            // Stop song
                            Player.Stop();

                            Console.WriteLine("Stopped playing");
                        }
                    }

                    // Apply song
                    currentsong = song;
                }
                else continue;
            }
        }

        static List<String> MapIndex = new List<String>();

        static void CreateIndex()
        {
            Console.WriteLine("Scanning for maps");

            // Saves all the maps in the folder to save time later searching in it
            foreach (String songfolder in Directory.GetDirectories(OsuFolder + "Songs")) {
                foreach (String map in Directory.GetFiles(songfolder))
                {
                    if (map.EndsWith(".osu"))
                    {
                        MapIndex.Add(map);
                        // Console.WriteLine("Added " + map);
                    }
                }
            }

            Console.WriteLine("Finished scanning");
        }

        static String getMapFile(String title)
        {
            Console.WriteLine(title);
            foreach (String mapfile in MapIndex)
            {
                    if (mapfile.Contains(title.Split('[')[0])) { 
                        Console.WriteLine("FOUND " + mapfile);
                        return mapfile;
                    }
                    // Console.WriteLine("compared " + mapfile + " with " + title);
                
            }
            return null;
        }
    }
}
