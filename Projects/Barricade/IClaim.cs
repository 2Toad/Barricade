/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

namespace Barricade
{
    public interface IClaim
    {
        /// <summary>
        /// The claim type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The value of the claim type.
        /// </summary>
        string Value { get; }
    }
}
