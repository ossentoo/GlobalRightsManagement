using System.Collections.Generic;
using GRMModels;

namespace GRMServices.Interfaces
{
    public interface IFileService
    {
        List<string> GetFileDataRows(string filePath);
    }
}