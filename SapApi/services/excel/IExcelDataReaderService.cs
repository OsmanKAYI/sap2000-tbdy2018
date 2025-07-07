using SAP2000.API.models.excel;
using System.Collections.Generic;

namespace SAP2000.API.services.excel
{
    public interface IExcelDataReaderService
    {
        List<ExcelColumnData> ReadColumnData(string filePath);
        List<ExcelBeamData> ReadBeamData(string filePath);
    }
}