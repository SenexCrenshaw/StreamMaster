import { skipToken } from '@reduxjs/toolkit/query/react';
import { MultiSelect, type MultiSelectChangeEvent } from "primereact/multiselect";
import { getChannelGroupMenuItem } from '../../common/common';
import { useStreamGroupChannelGroupGetAllChannelGroupsQuery, useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery, useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation, type ChannelGroupDto, type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg } from '../../store/iptvApi';

type StreamGroupChannelGroupsInputs = {
  readonly className?: string;
  readonly streamGroupId: number | undefined;
};

const StreamGroupChannelGroupsSelector = ({ className, streamGroupId }: StreamGroupChannelGroupsInputs) => {
  const { data: selectedData } = useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery(streamGroupId ?? skipToken);
  const { data: channelGroups } = useStreamGroupChannelGroupGetAllChannelGroupsQuery(streamGroupId ?? skipToken);

  const [syncStreamGroupChannelGroupsMutation, { isLoading }] = useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation();

  return (
    <div className={`"flex w-full ${className}"`}>
      <MultiSelect
        className='sm-selector flex w-full'
        disabled={isLoading}
        filter
        filterInputAutoFocus
        itemTemplate={(option) => getChannelGroupMenuItem(option.id, option.name + '  |  ' + option.totalCount)}
        maxSelectedLabels={1}
        onChange={async (e: MultiSelectChangeEvent) => {
          const toSend = {} as StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg;
          toSend.streamGroupId = streamGroupId;
          toSend.channelGroupIds = e.value;
          await syncStreamGroupChannelGroupsMutation(toSend)
            .then(() => {
            }).catch((error) => {
              console.error(error);
            });
        }}
        optionLabel="name"
        optionValue='id'
        options={channelGroups}
        placeholder="Groups"
        scrollHeight="40vh"
        selectedItemsLabel="{0} groups selected"
        showSelectAll={false}
        value={selectedData?.map((x: ChannelGroupDto) => x.id) ?? []}
      />
    </div>
  );
};


export default StreamGroupChannelGroupsSelector;
