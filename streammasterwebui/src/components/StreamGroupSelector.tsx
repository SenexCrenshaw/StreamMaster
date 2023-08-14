
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import React from 'react';
import { type StreamGroupsGetStreamGroupsApiArg } from '../store/iptvApi';
import { useStreamGroupsGetStreamGroupsQuery, type StreamGroupDto } from '../store/iptvApi';

export const StreamGroupSelector = (props: StreamGroupSelectorProps) => {

  const elementRef = React.useRef(null);

  const [selectedStreamGroup, setSelectedStreamGroup] = React.useState<StreamGroupDto>({} as StreamGroupDto);

  const streamGroups = useStreamGroupsGetStreamGroupsQuery({} as StreamGroupsGetStreamGroupsApiArg);

  const isDisabled = React.useMemo((): boolean => {
    if (streamGroups.isLoading) {
      return true;
    }

    return false;
  }, [streamGroups.isLoading]);


  const onDropdownChange = (sg: StreamGroupDto) => {
    if (!sg) return;

    setSelectedStreamGroup(sg);
    props?.onChange?.(sg);
  };

  const getOptions = React.useMemo((): SelectItem[] => {
    if (!streamGroups.data)
      return [
        {
          label: 'Loading...',
          value: {} as StreamGroupDto,
        } as SelectItem,
      ];

    const ret = streamGroups.data?.data?.map((a) => {
      return { label: a.name, value: a } as SelectItem;
    });

    ret.unshift({
      label: 'All',
      value: {} as StreamGroupDto,
    } as SelectItem);

    return ret;
  }, [streamGroups.data]);

  return (<div  >
    <Dropdown
      className="streamgroupselector"
      disabled={isDisabled}
      onChange={(e) => onDropdownChange(e.value)}
      options={getOptions}
      placeholder="Stream Group"
      ref={elementRef}
      value={selectedStreamGroup}
    />
  </div>
  );
};

StreamGroupSelector.displayName = 'StreamGroupSelector';
StreamGroupSelector.defaultProps = {

};
type StreamGroupSelectorProps = {
  onChange: ((value: StreamGroupDto) => void);
};
