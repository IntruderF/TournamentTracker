using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary
{
    public class PrizeModel
    {
        /// <summary>
        /// Represents the number of place.
        /// </summary>
        public int PlaceNumber { get; set; }
        /// <summary>
        /// Represents the place name.
        /// </summary>
        public string PlaceName { get; set; }
        /// <summary>
        /// Represents the prize amount.
        /// </summary>
        public decimal PrizeAmount { get; set; }
        /// <summary>
        /// Represent the prize percentage.
        /// </summary>
        public double PrizePercentage { get; set; }
    }
}
