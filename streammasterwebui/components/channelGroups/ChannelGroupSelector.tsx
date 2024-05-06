import { SMOverlay } from '@components/sm/SMOverlay';
import SMScroller from '@components/sm/SMScroller';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';

import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

type ChannelGroupSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
  readonly className?: string | undefined;
};

const ChannelGroupSelector = ({ enableEditMode = true, className, value, disabled, editable, onChange }: ChannelGroupSelectorProperties) => {
  const [selectedChannelGroup, setSelectedChannelGroup] = useState<ChannelGroupDto>();
  const [input, setInput] = useState<string | undefined>(undefined);
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);
  const { isSystemReady } = useSMContext();

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

  const loading = channelGroupQuery.isError || channelGroupQuery.isFetching || channelGroupQuery.isLoading || !channelGroupQuery.data || isSystemReady !== true;

  const buttonTemplate = useMemo(() => {
    if (input) return <div className="stringeditorbody-inputtext-dark text-xs text-container sm-hoover">{input}</div>;

    return <div className="stringeditorbody-inputtext-dark text-xs text-container text-white-alpha-40 sm-hoover">None</div>;
  }, [input]);

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center">
        <ProgressSpinner />
      </div>
    );
  }

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0">{input ?? 'Dummy'}</div>;
  }

  return (
    <SMOverlay buttonTemplate={buttonTemplate} title="CHANNEL GROUPS" widthSize="3" icon="pi-upload" buttonClassName="icon-green-filled" buttonLabel="EPG">
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
