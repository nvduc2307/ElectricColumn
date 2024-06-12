namespace CadDev.Extension.ICommand
{
    public interface ICadCommand
    {
        /// <summary>
        /// Execute a function in cad application
        /// </summary>
        public abstract void Execute();
    }
}