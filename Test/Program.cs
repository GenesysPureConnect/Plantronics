using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plantronics.UC.SpokesWrapper;

namespace Test
{
    class Program
    {
        static Spokes spokes;

        static void Main(string[] args)
        {
            spokes = Spokes.Instance;
            spokes.PutOn += spokes_PutOn;
            spokes.TakenOff += spokes_TakenOff;
            spokes.Near += new Spokes.NearEventHandler(spokes_Near);
            spokes.Far += new Spokes.FarEventHandler(spokes_Far);
            spokes.ProximityDisabled += new Spokes.ProximityDisabledEventHandler(spokes_ProximityDisabled);
            spokes.Connect("Spokes Quick Start");

            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();

            spokes.PutOn -= spokes_PutOn;
            spokes.TakenOff -= spokes_TakenOff;
            spokes.Near -= spokes_Near;
            spokes.Far -= spokes_Far;
            spokes.ProximityDisabled -= spokes_ProximityDisabled;
            spokes.Disconnect();
        }

        static void spokes_Far(object sender, EventArgs e)
        {
            Console.WriteLine("Headset is far from PC");
        }

        static void spokes_Near(object sender, EventArgs e)
        {
            Console.WriteLine("Headset is near to PC");
        }

        static void spokes_ProximityDisabled(object sender, EventArgs e)
        {
            // When the mobile disconnects from headset we get a "ProximityDisabled" event
            // that we can use to recognise this... 
            // Note: The wrapper will automatically re-register for proximity when this occurs
            Console.WriteLine("Mobile phone out of range of headset (or mobile powered off)");
        }

        static void spokes_TakenOff(object sender, WearingStateArgs e)
        {
            Console.WriteLine("Headset is not worn" + (e.m_isInitialStateEvent ? " (initial state)" : ""));
        }

        static void spokes_PutOn(object sender, WearingStateArgs e)
        {
            Console.WriteLine("Headset is worn" + (e.m_isInitialStateEvent ? " (initial state)" : ""));
        }
    }
}
