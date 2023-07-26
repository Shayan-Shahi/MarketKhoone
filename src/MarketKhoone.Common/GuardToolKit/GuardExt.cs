namespace MarketKhoone.Common.GuardToolKit
{
    public static class GuardExt
    {
        /// <summary>
        /// Checks if the argument is null
        /// </summary>
        /// <param name="o"></param>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CheckArgumentIsNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }
    }
}
