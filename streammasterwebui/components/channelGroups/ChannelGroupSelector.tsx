import { SMOverlay } from '@components/sm/SMOverlay';
import SMScroller from '@components/sm/SMScroller';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';

import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useCallback, useEffect, useMemo, useState } from 'react';

type ChannelGroupSelectorProperties = {
  readonly darkBackGround?: boolean;
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly label?: string;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
  // readonly className?: string | undefined;
};

const ChannelGroupSelector = ({
  enableEditMode = true,
  // className,
  darkBackGround = false,
  label,
  value,
  disabled,
  editable,
  onChange
}: ChannelGroupSelectorProperties) => {
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
    return <div className="text-xs pl-2 text-container">{option.Name}</div>;
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
    if (input)
      return (
        <div className="sm-channelgroup-selector">
          <div className="text-container ">{input}</div>
        </div>
      );

    return (
      <div className="sm-channelgroup-selector ">
        <div className="text-container ">None</div>
      </div>
    );
  }, [input]);

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center">
        <ProgressSpinner />
      </div>
    );
  }

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0 text-container">{input ?? 'Dummy'}</div>;
  }

  return (
    <>
      <div className="stringeditor flex flex-column align-items-start">
        {label && (
          <>
            <label className="pl-15">{label.toUpperCase()}</label>
            <div className="pt-small" />
          </>
        )}
      </div>
      <div className={darkBackGround ? 'sm-input-border-dark p-0 input-height' : 'p-0 input-height'}>
        <SMOverlay buttonTemplate={buttonTemplate} title="GROUPS" widthSize="3" icon="pi-chevron-down">
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
      </div>
    </>
  );
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default React.memo(ChannelGroupSelector);
