/* eslint-disable @typescript-eslint/no-unused-vars */
import { skipToken } from '@reduxjs/toolkit/query/react';
import { MultiSelect, type MultiSelectChangeEvent } from "primereact/multiselect";
import { useChannelGroupsGetAllChannelGroupsQuery, useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery, useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation, type ChannelGroupDto, type StreamGroupChannelGroupSyncStreamGroupChannelGroupsApiArg } from "../../store/iptvApi";

type StreamGroupChannelGroupsInputs = {
  readonly streamGroupId: number | undefined;
};

const StreamGroupChannelGroupsSelector = ({ streamGroupId }: StreamGroupChannelGroupsInputs) => {
  const { data: selectedData } = useStreamGroupChannelGroupGetChannelGroupsFromStreamGroupQuery(streamGroupId ?? skipToken);
  const { data: channelGroups } = useChannelGroupsGetAllChannelGroupsQuery();

  const [syncStreamGroupChannelGroupsMutation] = useStreamGroupChannelGroupSyncStreamGroupChannelGroupsMutation();

  const itemTemplate = (option: ChannelGroupDto) => {
    return (
      <div className="align-items-center gap-2">
        <span>{option.name}</span>
      </div>
    );
  };

  return (
    <div className="flex">
      <MultiSelect
        className="p-column-filter text-xs"
        filter
        filterInputAutoFocus
        itemTemplate={itemTemplate}
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
