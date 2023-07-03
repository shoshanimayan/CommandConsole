namespace Console
{
    public interface IParameterConverter
    {
        public abstract object Convert(string userValue);
    }
}