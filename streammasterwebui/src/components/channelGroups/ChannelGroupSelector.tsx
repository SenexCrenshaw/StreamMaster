import React, { useState, useMemo, useCallback } from "react";
import { Dropdown } from "primereact/dropdown";
import { Button } from "primereact/button";
import { useChannelGroupsGetChannelGroupNamesQuery } from "../../store/iptvApi";
import { getTopToolOptions } from "../../common/common";
import { ResetLogoIcon } from "../../common/icons";
import ChannelGroupAddDialog from "./ChannelGroupAddDialog";

type SelectItem = {
  label: string;
  value: string;
};

type ChannelGroupSelectorProps = {
  readonly onChange: (value: string) => void;
  readonly resetValue?: string;
  readonly value?: string;
}

const ChannelGroupSelector: React.FC<ChannelGroupSelectorProps> = ({ onChange, resetValue, value }) => {
  const channelGroupNamesQuery = useChannelGroupsGetChannelGroupNamesQuery();
  const [channelGroup, setChannelGroup] = useState<string | undefined>(value);

  // Update channel group when prop value changes
  useMemo(() => {
    if (value) {
      setChannelGroup(value);
    }
  }, [value]);

  // Prepare dropdown options from API data
  const options: SelectItem[] = useMemo(() => {
    if (!channelGroupNamesQuery.data) return [];

    return channelGroupNamesQuery.data.map((cg) => ({ label: cg, value: cg }));
  }, [channelGroupNamesQuery.data]);

  const onChannelGroupAddDialogClose = (newGroupName: string) => {
    setChannelGroup(newGroupName);
    onChange(newGroupName);
  };

  const handleResetClick = () => {
    if (resetValue) {
      setChannelGroup(resetValue);
      onChange(resetValue);
    }
  };

  const footerTemplate = () => (
    <div className="p-1 align-items-center justify-content-center">
      <hr />
      <div className='flex gap-2 align-items-center justify-content-end'>

        {value !== resetValue && resetValue && (
          <Button
            icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
            onClick={handleResetClick}
            rounded
            severity="warning"
            size="small"
            tooltip="Reset Group"
            tooltipOptions={getTopToolOptions}
          />
        )}
        <ChannelGroupAddDialog onAdd={onChannelGroupAddDialogClose} />
      </div>
    </div>
  );

  const selectedTemplate = useCallback((option: SelectItem) => {
    if (!option) return;

    return <div className='flex h-full justify-content-start align-items-center p-0 m-0'>{option.label}</div>;
  }, []);

  return (
    <div className="iconSelector flex w-full justify-content-start align-items-center">
      <Dropdown
        className='iconSelector p-0 m-0 w-full'
        filter
        filterInputAutoFocus
        onChange={(e) => onChange(e.value)}
        options={options}
        panelFooterTemplate={footerTemplate}
        placeholder="No Group"
        style={{
          backgroundColor: 'var(--mask-bg)',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          whiteSpace: 'nowrap',
        }}
        value={channelGroup}
        valueTemplate={selectedTemplate}
      />
    </div>
  );
}

ChannelGroupSelector.displayName = 'Channel Group Dropdown';

export default React.memo(ChannelGroupSelector);
