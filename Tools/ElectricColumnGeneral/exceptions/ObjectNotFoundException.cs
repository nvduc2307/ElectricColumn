namespace CadDev.Tools.ElectricColumnGeneral.exceptions
{
    public class ObjectNotFoundException : Exception
    {
        public const string MessageError = "Object Not Found";
        public ObjectNotFoundException() : base(MessageError) { }
    }
}
