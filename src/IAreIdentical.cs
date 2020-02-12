﻿namespace Wanderer
{
    public interface IAreIdentical<T>
    {
        /// <summary>
        /// Returns true if the user would consider 2 objects the same e.g. 2 "Rusty Broken Gun"
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool AreIdentical(T other);
    }
}