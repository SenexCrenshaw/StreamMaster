using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.EPGFiles.Commands;
using StreamMaster.Application.EPGFiles.Queries;

namespace StreamMaster.Application.EPGFiles
{
    public partial class EPGFilesController(ISender Sender, ILogger<EPGFilesController> _logger) : ApiControllerBase, IEPGFilesController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<EPGColorDto>>> GetEPGColors()
        {
            try
            {
            APIResponse<List<EPGColorDto>> ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetEPGColors.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetEPGColors.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<EPGFilePreviewDto>>> GetEPGFilePreviewById(int Id)
        {
            try
            {
            APIResponse<List<EPGFilePreviewDto>> ret = await Sender.Send(new GetEPGFilePreviewByIdRequest(Id)).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetEPGFilePreviewById.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetEPGFilePreviewById.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<int>> GetEPGNextEPGNumber()
        {
            try
            {
            APIResponse<int> ret = await Sender.Send(new GetEPGNextEPGNumberRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetEPGNextEPGNumber.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetEPGNextEPGNumber.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> CreateEPGFile(CreateEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteEPGFile(DeleteEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> ProcessEPGFile(ProcessEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> RefreshEPGFile(RefreshEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> UpdateEPGFile(UpdateEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
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
             APIResponse<List<EPGColorDto>> ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<EPGFilePreviewDto>> GetEPGFilePreviewById(int Id)
        {
             APIResponse<List<EPGFilePreviewDto>> ret = await Sender.Send(new GetEPGFilePreviewByIdRequest(Id)).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<int> GetEPGNextEPGNumber()
        {
             APIResponse<int> ret = await Sender.Send(new GetEPGNextEPGNumberRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<DefaultAPIResponse> CreateEPGFile(CreateEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteEPGFile(DeleteEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> ProcessEPGFile(ProcessEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> RefreshEPGFile(RefreshEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> UpdateEPGFile(UpdateEPGFileRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
