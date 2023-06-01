import { Dropdown } from "primereact/dropdown";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import { Button } from "primereact/button";
import { getTopToolOptions } from "../common/common";
import ChannelGroupAddDialog from "./ChannelGroupAddDialog";
import { ResetLogoIcon } from "../common/icons";
import { type SelectItem } from "primereact/selectitem";
import { Toast } from 'primereact/toast';

const ChannelGroupSelector = (props: ChannelGroupSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery();

  const [channelGroup, setChannelGroup] = React.useState<string>('');

  React.useMemo(() => {

    if (props.value !== undefined) {
      setChannelGroup(props.value);
    }

  }, [props.value]);

  const options = React.useMemo(() => {
    if (!channelGroupsQuery.data) return [];

    return channelGroupsQuery.data.map((cg) => {
      return { label: cg.name, value: cg.name };
    });

  }, [channelGroupsQuery.data]);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars, @typescript-eslint/no-explicit-any
  const filterTemplate = React.useCallback((filterOptions: any) => {
    return (
      <div className="flex gap-2 align-items-center">
        <ChannelGroupAddDialog />
        {filterOptions.element}
        {(props.resetValue !== undefined && props.value !== props.resetValue) &&
          < Button
            icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
            onClick={() => {
              if (props.resetValue !== undefined) {
                setChannelGroup(props.resetValue);
              }
            }
            }
            rounded
            severity="warning"
            size="small"
            tooltip="Reset Group"
            tooltipOptions={getTopToolOptions}
          />
        }
      </div>
    );
  }, [props.resetValue, props.value]);


  const selectedTemplate = React.useCallback((option: SelectItem) => {
    if (option === null) {
      return;
    }

    return (
      <div className='flex h-full justify-content-start align-items-center p-0 m-0'>
        {option.label}
      </div>
    );
  }, []);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <div className="iconSelector flex w-full justify-content-start align-items-center">
        <Dropdown
          className='iconSelector p-0 m-0 w-full'
          filter
          filterTemplate={filterTemplate}
          onChange={((e) => props.onChange(e.value))}
          options={options}
          placeholder="Group"
          style={{
            ...{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            },
          }}
          value={channelGroup}
          valueTemplate={selectedTemplate}
        />
      </div>
    </>
  );
}


ChannelGroupSelector.displayName = 'Channel Group Dropdown';
ChannelGroupSelector.defaultProps = {

};

export type ChannelGroupSelectorProps = {
  onChange: ((e: string) => void);
  resetValue?: string | undefined;
  value: string | undefined;
};

export default React.memo(ChannelGroupSelector);
