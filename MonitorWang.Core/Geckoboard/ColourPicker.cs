using System;
using System.Collections.Generic;
using System.Drawing;

namespace MonitorWang.Core.Geckoboard
{
    public interface IColourPicker
    {
        ColourPicker.DisplayColour Next(string label);
    }

    /// <summary>
    /// This class provides random colours for use in charts/graphs etc
    /// </summary>
    public class ColourPicker
    {
        public class DisplayColour
        {
            private Color myColour;

            public static DisplayColour FromKnownColour(Color colour)
            {
                return new DisplayColour
                           {
                               myColour = colour
                           };
            }

            public static DisplayColour FromHexString(string colour)
            {
                try
                {
                    var knownColour = (KnownColor)Enum.Parse(typeof(KnownColor), colour, true);
                    return new DisplayColour
                               {
                                   myColour = Color.FromKnownColor(knownColour)
                               };
                }
                catch
                {
                    
                }
                

                var alpha = Convert.ToInt32("0x" + colour.Substring(0, 2));
                var red = Convert.ToInt32("0x" + colour.Substring(2, 2));
                var green = Convert.ToInt32("0x" + colour.Substring(4, 2));
                var blue = Convert.ToInt32("0x" + colour.Substring(6, 2));

                return FromArgb(alpha, red, green, blue);                
            }

            public static DisplayColour FromArgb(int alpha, int red, int green, int blue)
            {
                return new DisplayColour
                {
                    myColour = Color.FromArgb(alpha, red, green, blue)
                };                
            }

            public override string ToString()
            {
                return string.Format("{0:X2}{1:X2}{2:X2}", myColour.R, myColour.G ,myColour.B);
            }
        }

        protected static IColourPicker myInstance;

        static ColourPicker()
        {
            myInstance = Container.Resolve<IColourPicker>();
        }

        public static string Next(string label)
        {
            return myInstance.Next(label).ToString();
        }
    }

    public class DefaultColourPicker : IColourPicker
    {
        protected static Random myRandomiser;
        protected Dictionary<string, ColourPicker.DisplayColour> myCache;
        protected List<string> myRemainingFavourites;

        /// <summary>
        /// This is a set of colours reserved for specific labels
        /// </summary>
        public Dictionary<string, string> Reserved { get; set; }

        private List<string> myFavourites;
        public List<string> Favourites
        {
            get { return myFavourites; }
            set
            {
                myFavourites = value;
                myRemainingFavourites = new List<string>(myFavourites);
            }
        }

        public DefaultColourPicker()
        {
            myRandomiser = new Random();          
            myCache = new Dictionary<string, ColourPicker.DisplayColour>();

            Favourites = new List<string>();
            Reserved = new Dictionary<string, string>();
        }

        public ColourPicker.DisplayColour Next(string label)
        {
            ColourPicker.DisplayColour colour;

            // check cache...
            if (myCache.ContainsKey(label))
                return myCache[label];

            // check reserved
            if (Reserved.ContainsKey(label))
            {
                colour = ColourPicker.DisplayColour.FromHexString(Reserved[label]);
                myCache.Add(label, colour);
                return colour;
            }
            
            // use the next favourite
            if (myRemainingFavourites.Count > 0)
            {                    
                colour = ColourPicker.DisplayColour.FromHexString(myRemainingFavourites[0]);
                myCache.Add(label, colour);
                myRemainingFavourites.RemoveAt(0);
                return colour;                
            }

            // random
            colour = ColourPicker.DisplayColour.FromArgb(
                myRandomiser.Next(100, 255),
                myRandomiser.Next(0, 200),
                myRandomiser.Next(0, 200),
                myRandomiser.Next(0, 200));
            myCache.Add(label, colour);
            return colour;
        }
    }
}