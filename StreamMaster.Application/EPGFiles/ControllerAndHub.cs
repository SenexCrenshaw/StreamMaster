using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;

namespace StreamMaster.Application.EPGFiles
{
    public partial class EPGFilesController(ISender Sender) : ApiControllerBase, IEPGFilesController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<List<EPGColorDto>> GetEPGColors()
        {
            List<EPGColorDto> ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
            return ret;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id)
        {
            List<EPGFilePreviewDto> ret = await Sender.Send(new GetEPGFilePreviewByIdRequest(Id)).ConfigureAwait(false);
            return ret;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<int> GetEPGNextEPGNumber()
        {
            int ret = await Sender.Send(new GetEPGNextEPGNumberRequest()).ConfigureAwait(false);
            return ret;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<EPGFileDto?>> CreateEPGFile(CreateEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<int>> DeleteEPGFile(DeleteEPGFileRequest request)
        {
            int ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<EPGFileDto?>> ProcessEPGFile(ProcessEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<EPGFileDto?>> RefreshEPGFile(RefreshEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<EPGFileDto?>> UpdateEPGFile(UpdateEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IEPGFilesHub
    {
        public async Task<List<EPGColorDto>> GetEPGColors()
        {
            List<EPGColorDto> ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id)
        {
            List<EPGFilePreviewDto> ret = await Sender.Send(new GetEPGFilePreviewByIdRequest(Id)).ConfigureAwait(false);
            return ret;
        }

        public async Task<int> GetEPGNextEPGNumber()
        {
            int ret = await Sender.Send(new GetEPGNextEPGNumberRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<EPGFileDto?> CreateEPGFile(CreateEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<int> DeleteEPGFile(DeleteEPGFileRequest request)
        {
            int ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<EPGFileDto?> ProcessEPGFile(ProcessEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<EPGFileDto?> RefreshEPGFile(RefreshEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<EPGFileDto?> UpdateEPGFile(UpdateEPGFileRequest request)
        {
            EPGFileDto? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
