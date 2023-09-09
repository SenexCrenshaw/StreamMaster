import { skipToken } from '@reduxjs/toolkit/query/react';
import { MultiSelect, type MultiSelectChangeEvent } from "primereact/multiselect";
import { getChannelGroupMenuItem } from '../../common/common';
import { useStreamGroupChannelGroupGetAllChannelGroupsQuery, useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery, useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation, type ChannelGroupDto, type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg } from '../../store/iptvApi';

type StreamGroupChannelGroupsInputs = {
  readonly streamGroupId: number | undefined;
};

const StreamGroupChannelGroupsSelector = ({ streamGroupId }: StreamGroupChannelGroupsInputs) => {
  const { data: selectedData } = useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery(streamGroupId ?? skipToken);
  const { data: channelGroups } = useStreamGroupChannelGroupGetAllChannelGroupsQuery(streamGroupId ?? skipToken);

  const [syncStreamGroupChannelGroupsMutation, { isLoading }] = useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation();

  return (
    <div className="flex">
      <MultiSelect
        className="sm-selector"
        disabled={isLoading}
        filter
        filterInputAutoFocus
        itemTemplate={(option) => getChannelGroupMenuItem(option.id, option.name)}
        maxSelectedLabels={1}
        onChange={async (e: MultiSelectChangeEvent) => {

          const toSend = {} as StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg;
          toSend.streamGroupId = streamGroupId;
          toSend.channelGroupIds = e.value.map((x: ChannelGroupDto) => x.id);

          await syncStreamGroupChannelGroupsMutation(toSend)
            .then(() => {
            }).catch((error) => {
              console.error(error);
            });

        }}
        optionLabel="name"
        options={channelGroups}
        placeholder="Groups"
        scrollHeight="40vh"
        selectedItemsLabel="{0} groups selected"
        showSelectAll={false}
        value={selectedData}
      />
    </div>
  );
};


export default StreamGroupChannelGroupsSelector;
