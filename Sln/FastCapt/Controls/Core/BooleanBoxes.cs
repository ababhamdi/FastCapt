namespace FastCapt.Controls.Core
{
    internal class BooleanBoxes
    {
        internal static object False = false;
        internal static object True = true;

        internal static object Box(bool value)
        {
            return value ? True : False;
        }
    }
}
