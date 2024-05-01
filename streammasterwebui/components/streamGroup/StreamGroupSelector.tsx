import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { Dropdown } from 'primereact/dropdown';
import { useMemo } from 'react';

interface StreamGroupSelectorProperties {
  readonly selectedStreamGroup: StreamGroupDto;
  readonly onChange: (value: StreamGroupDto) => void;
}

export const StreamGroupSelector = ({ onChange, selectedStreamGroup }: StreamGroupSelectorProperties) => {
  const { data, isLoading } = useGetStreamGroups();

  const isDisabled = useMemo((): boolean => {
    if (isLoading) {
      return true;
    }

    return false;
  }, [isLoading]);

  const onDropdownChange = (sg: StreamGroupDto) => {
    if (!sg) return;
    if (sg.Id === selectedStreamGroup.Id) return;
    onChange?.(sg);
  };

  return (
    <div>
      <Dropdown
        className="sm-streamgroupselector"
        dataKey="Id"
        disabled={isDisabled}
        onChange={(e) => onDropdownChange(e.value)}
        options={data}
        optionLabel="Name"
        placeholder="Stream Group"
        value={selectedStreamGroup}
      />
    </div>
  );
};

StreamGroupSelector.displayName = 'StreamGroupSelector';
