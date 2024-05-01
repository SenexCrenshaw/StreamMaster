import { SMOverlay } from '@components/sm/SMOverlay';
import SMScroller from '@components/sm/SMScroller';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import useGetIsSystemReady from '@lib/smAPI/Settings/useGetIsSystemReady';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useCallback, useEffect, useState } from 'react';

type ChannelGroupSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
};

const ChannelGroupSelector = ({ enableEditMode = true, value, disabled, editable, onChange }: ChannelGroupSelectorProperties) => {
  const [selectedChannelGroup, setSelectedChannelGroup] = useState<ChannelGroupDto>();
  const [input, setInput] = useState<string | undefined>(undefined);
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);
  const getIsSystemReady = useGetIsSystemReady();

  const channelGroupQuery = useGetChannelGroups();

  useEffect(() => {
    if (!originalInput || originalInput !== value) {
      setOriginalInput(value);
      if (value) {
        setInput(value);
        const found = channelGroupQuery.data?.find((x) => x.Name === value);
        if (found) {
          setSelectedChannelGroup(found);
        }
      }
    }
  }, [value, originalInput, channelGroupQuery.data]);

  const itemTemplate = useCallback((option: ChannelGroupDto): JSX.Element => {
    return <div className="text-xs pl-2">{option.Name}</div>;
  }, []);

  const handleOnChange = (group: ChannelGroupDto) => {
    if (!group) {
      return;
    }

    setInput(group.Name);

    onChange && onChange(group.Name);
  };

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{input ?? 'Dummy'}</div>;
  }

  const loading =
    channelGroupQuery.isError || channelGroupQuery.isFetching || channelGroupQuery.isLoading || !channelGroupQuery.data || getIsSystemReady.data !== true;

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center">
        <ProgressSpinner />
      </div>
    );
  }

  return (
    <SMOverlay
      buttonTemplate={<div>{selectedChannelGroup?.Name ?? ''}</div>}
      title="CHANNEL GROUPS"
      widthSize="5"
      icon="pi-upload"
      buttonClassName="icon-green-filled"
      buttonLabel="EPG"
    >
      <SMScroller
        data={channelGroupQuery.data}
        onChange={(e) => handleOnChange(e)}
        dataKey="Name"
        filter
        filterBy="Name"
        itemSize={26}
        itemTemplate={itemTemplate}
        scrollHeight={250}
        value={selectedChannelGroup}
      />
    </SMOverlay>
  );
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default React.memo(ChannelGroupSelector);
