
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { useRef, useState, useMemo } from 'react';
import { type StreamGroupDto, type StreamGroupsGetStreamGroupsApiArg } from '../../store/iptvApi';
import { useStreamGroupsGetStreamGroupsQuery } from '../../store/iptvApi';

export const StreamGroupSelector = (props: StreamGroupSelectorProps) => {

  const elementRef = useRef(null);

  const [selectedStreamGroup, setSelectedStreamGroup] = useState<StreamGroupDto>({} as StreamGroupDto);

  const streamGroups = useStreamGroupsGetStreamGroupsQuery({} as StreamGroupsGetStreamGroupsApiArg);

  const isDisabled = useMemo((): boolean => {
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

  const getOptions = useMemo((): SelectItem[] => {
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
