import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import useGetIsSystemReady from '@lib/smAPI/Settings/useGetIsSystemReady';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { ProgressSpinner } from 'primereact/progressspinner';
import { classNames } from 'primereact/utils';
import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';

type ChannelGroupSelectorProperties = {
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly editable?: boolean | undefined;
  readonly value?: string;
  readonly onChange?: (value: string) => void;
};

const ChannelGroupSelector = ({ enableEditMode = true, value, disabled, editable, onChange }: ChannelGroupSelectorProperties) => {
  const dropDownRef = useRef<Dropdown>(null);

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

  const itemTemplate = useCallback(
    (option: ChannelGroupDto): JSX.Element => {
      if (!option) {
        return <div>{input}</div>;
      }
      return <div>{option.Name}</div>;
    },
    [input]
  );

  const valueTemplate = useCallback(
    (option: ChannelGroupDto): JSX.Element => {
      if (!option) {
        return <div>{input}</div>;
      }
      return <div>{option.Name}</div>;
    },
    [input]
  );

  const handleOnChange = (group: string) => {
    if (!group) {
      return;
    }

    setInput(group);

    dropDownRef.current?.hide();
    // setOriginalInput(undefined);
    onChange && onChange(group);
  };

  const options = useMemo(() => {
    if (!channelGroupQuery.data) {
      return undefined;
    }

    return channelGroupQuery.data;
  }, [channelGroupQuery.data]);

  const className = classNames('max-w-full w-full channelGroupSelector', {
    'p-disabled': disabled
  });

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
    <div className="sm-input flex align-contents-center w-full min-w-full h-full ">
      <Dropdown
        className={className}
        disabled={loading}
        filterInputAutoFocus
        filter
        filterBy="Name"
        itemTemplate={itemTemplate}
        loading={loading}
        onChange={(e) => {
          handleOnChange(e?.value?.Name);
        }}
        onHide={() => {}}
        optionLabel="Name"
        options={options}
        panelClassName="sm-channelgroup-editor-panel"
        placeholder="placeholder"
        ref={dropDownRef}
        resetFilterOnHide
        showFilterClear={false}
        value={selectedChannelGroup}
        valueTemplate={valueTemplate}
        virtualScrollerOptions={{
          itemSize: 26
        }}
      />
    </div>
  );
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default React.memo(ChannelGroupSelector);
