
import { StreamGroupDto, StreamGroupsGetPagedStreamGroupsApiArg, useStreamGroupsGetPagedStreamGroupsQuery } from '@/lib/iptvApi';
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { useMemo, useRef, useState } from 'react';

export const StreamGroupSelector = (props: StreamGroupSelectorProps) => {

  const elementRef = useRef(null);

  const [selectedStreamGroup, setSelectedStreamGroup] = useState<StreamGroupDto>({} as StreamGroupDto);

  const streamGroups = useStreamGroupsGetPagedStreamGroupsQuery({} as StreamGroupsGetPagedStreamGroupsApiArg
  );

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

type StreamGroupSelectorProps = {
  readonly onChange: ((value: StreamGroupDto) => void);
};
