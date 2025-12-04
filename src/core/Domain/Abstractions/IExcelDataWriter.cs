using DocumentFormat.OpenXml;

namespace Domain.Abstractions
{
    public interface IExcelDataWriter
    {
        void WriteRow(OpenXmlWriter writer, string[] values);
    }
}
