using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Exceptions;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupSMChannelLinkRepository(ILogger<StreamGroupSMChannelLinkRepository> logger, IRepositoryContext repositoryContext, IRepositoryWrapper repository)
    : RepositoryBase<StreamGroupSMChannelLink>(repositoryContext, logger), IStreamGroupSMChannelLinkRepository
{
    public async Task AddSMChannelsToStreamGroupAsync(int StreamGroupId, List<int> SMChannelIds, bool? skipSave = false)
    {
        foreach (int smChannelId in SMChannelIds)
        {
            AddSMChannelToStreamGroupInternal(StreamGroupId, smChannelId);

        }

        if (skipSave == false)
        {
            await SaveChangesAsync();
        }

    }

    private void AddSMChannelToStreamGroupInternal(int StreamGroupId, int SMChannelId)
    {
        // Check if the entity is already tracked by EF Core
        //EntityEntry<StreamGroupSMChannelLink>? existingLink = repositoryContext.ChangeTracker.Entries<StreamGroupSMChannelLink>()
        //    .FirstOrDefault(e => e.Entity.SMChannelId == SMChannelId && e.Entity.StreamGroupId == StreamGroupId);

        //EntityEntry<StreamGroupSMChannelLink>? existingLink = repositoryContext.StreamGroupSMChannelLinks.has
        //  .FirstOrDefault(e => e.Entity.SMChannelId == SMChannelId && e.Entity.StreamGroupId == StreamGroupId);

        // If the link isn't tracked, create it
        //if (existingLink == null)
        //{
        //StreamGroupSMChannelLink link = new()
        //{
        //    SMChannelId = SMChannelId,
        //    StreamGroupId = StreamGroupId,
        //    IsReadOnly = false,
        //    Rank = 0
        //};

        //Create(link);


        string sql = $"INSERT INTO public.\"StreamGroupSMChannelLink\" (\"SMChannelId\", \"StreamGroupId\", \"IsReadOnly\", \"Rank\") VALUES ({SMChannelId}, {StreamGroupId}, false, 0);";

        repositoryContext.ExecuteSqlRaw(sql);
        //}
        //else
        //{
        //    // Optionally, handle the situation where the link is already tracked
        //    // For example, you might update the tracked entity if necessary
        //    //existingLink.Entity.IsReadOnly = false; // Example update
        //    //Update(existingLink.Entity);
        //}
    }

    public async Task<APIResponse> AddSMChannelToStreamGroup(int StreamGroupId, int SMChannelId, bool? skipSave = false, bool? skipCheck = false)
    {
        if (repository.StreamGroup.GetStreamGroup(StreamGroupId) == null || repository.SMChannel.GetSMChannel(SMChannelId) == null)
        {
            throw new APIException($"Channel with Id {SMChannelId} or Stream Group with Id {StreamGroupId} not found");
        }

        if (skipCheck == false)
        {
            if (Any(a => a.StreamGroupId == StreamGroupId && a.SMChannelId == SMChannelId))
            {
                return APIResponse.Success;
            }
        }

        StreamGroupSMChannelLink link = new()
        {
            SMChannelId = SMChannelId,
            StreamGroupId = StreamGroupId,
            IsReadOnly = false,
            Rank = 0
        };

        Create(link);
        if (skipSave == false)
        {
            await SaveChangesAsync();
        }

        return APIResponse.Success;
    }

    public async Task<APIResponse> RemoveSMChannelFromStreamGroup(int StreamGroupId, int SMChannelId)
    {
        IQueryable<StreamGroupSMChannelLink> toDelete = GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.StreamGroupId == StreamGroupId);
        if (!toDelete.Any())
        {
            throw new APIException($"Channel with Id {SMChannelId} or Stream Group with Id {StreamGroupId} not found");
        }

        await BulkDeleteAsync(toDelete);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> RemoveSMChannelsFromStreamGroup(int StreamGroupId, List<int> SMChannelIds)
    {
        IQueryable<StreamGroupSMChannelLink> toDelete = GetQuery(true).Where(a => SMChannelIds.Contains(a.SMChannelId) && a.StreamGroupId == StreamGroupId);
        if (!toDelete.Any())
        {
            return APIResponse.Success;
        }

        await BulkDeleteAsync(toDelete);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public override IQueryable<StreamGroupSMChannelLink> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMChannel)
            : base.GetQuery(tracking).Include(a => a.SMChannel).AsNoTracking();
    }


}