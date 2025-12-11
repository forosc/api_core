using DocumentFormat.OpenXml;

namespace Application.Interfaces
{
    public interface IExcelDataWriter
    {
        void WriteRow(OpenXmlWriter writer, string[] values);
    }
}
