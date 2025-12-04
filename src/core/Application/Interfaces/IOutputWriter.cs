namespace Application.Interfaces
{
    public interface IOutputWriter
    {
        void WriteRow(string[] values);
        void Close();
    }
}
