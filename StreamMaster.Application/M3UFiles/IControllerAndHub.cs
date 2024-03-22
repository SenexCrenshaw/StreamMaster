using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.M3UFiles.Commands;

namespace StreamMaster.Application.M3UFiles
{
    public interface IM3UFilesController
    {        
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IM3UFilesHub
    {
        Task<DefaultAPIResponse?> ProcessM3UFile(ProcessM3UFileRequest request);
    }
}
