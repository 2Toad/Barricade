/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

namespace Barricade
{
    public class Claim : IClaim
    {
        /// <summary>
        /// The claim type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The value of the claim type.
        /// </summary>
        public string Value { get; set; }
    }
}
