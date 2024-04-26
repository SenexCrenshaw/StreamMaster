import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import { useMemo } from 'react';

interface StreamGroupSelectorProperties {
  readonly selectedStreamGroup: StreamGroupDto;
  readonly onChange: (value: StreamGroupDto) => void;
}

export const StreamGroupSelector = (props: StreamGroupSelectorProperties) => {
  const { data, isLoading } = useGetStreamGroups();

  const isDisabled = useMemo((): boolean => {
    if (isLoading) {
      return true;
    }

    return false;
  }, [isLoading]);

  const onDropdownChange = (sg: StreamGroupDto) => {
    if (!sg) return;
    if (sg.Id === props.selectedStreamGroup.Id) return;
    props?.onChange?.(sg);
  };

  const getOptions = useMemo((): SelectItem[] => {
    if (!data) {
      return [
        {
          label: 'Loading...',
          value: {} as StreamGroupDto
        } as SelectItem
      ];
    }

    const returnValue = data?.map((a) => ({ label: a.Name, value: a } as SelectItem));
    return returnValue;
  }, [data]);

  return (
    <div>
      <Dropdown
        className="sm-streamgroupselector"
        dataKey="Id"
        disabled={isDisabled}
        onChange={(e) => onDropdownChange(e.value)}
        options={getOptions}
        placeholder="Stream Group"
        value={props.selectedStreamGroup}
      />
    </div>
  );
};

StreamGroupSelector.displayName = 'StreamGroupSelector';
