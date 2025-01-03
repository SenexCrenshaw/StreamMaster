using StreamMaster.Domain.Models;
using StreamMaster.Domain.XmltvXml;

namespace StreamMaster.EPG
{
    /// <summary>
    /// Represents a service that can match an SMChannel to the best possible EPG channel.
    /// </summary>
    public interface IEpgMatcher
    {
        /// <summary>
        /// Attempts to find the best matching EPG station for a given <see cref="SMChannel"/>.
        /// First tries to match by EPGID. If not found, attempts a fuzzy match by TVGName.
        /// </summary>
        /// <param name="channel">The SMChannel to match.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// The matched <see cref="StationChannelName"/> if found; otherwise, returns null.
        /// </returns>
        Task<StationChannelName?> MatchAsync(SMChannel channel, CancellationToken cancellationToken);
    }
}