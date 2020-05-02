using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.Models
{
    public class PersonModel
    {
        /// <summary>
        /// The unique identifier for the prize.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Represents the first name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Represents the last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Represents the email address.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Represents the cellphone number.
        /// </summary>
        public string CellphoneNumber { get; set; }
        
        public string FullName
        {
            get
            {
                return $"{ FirstName } { LastName }";
            }
        }
    }
}
