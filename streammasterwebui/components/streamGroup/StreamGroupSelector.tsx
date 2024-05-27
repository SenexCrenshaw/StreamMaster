import SMDropDown from '@components/sm/SMDropDown';
import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { useMemo } from 'react';

interface StreamGroupSelectorProperties {
  readonly selectedStreamGroup: StreamGroupDto | undefined;
  readonly onChange: (value: StreamGroupDto) => void;
}

export const StreamGroupSelector = ({ onChange, selectedStreamGroup }: StreamGroupSelectorProperties) => {
  const { data, isLoading } = useGetStreamGroups();

  const onDropdownChange = (sg: StreamGroupDto) => {
    if (!sg) return;
    if (selectedStreamGroup && sg.Id === selectedStreamGroup.Id) return;
    onChange?.(sg);
  };

  const buttonTemplate = useMemo(() => {
    return <div className="text-container ">{selectedStreamGroup?.Name ?? 'Select Stream Group'}</div>;
  }, [selectedStreamGroup?.Name]);

  return (
    <SMDropDown
      buttonDarkBackground
      buttonTemplate={buttonTemplate}
      data={data}
      dataKey="Id"
      isLoading={isLoading}
      itemTemplate={(option: StreamGroupDto) => option?.Name}
      filter
      filterBy="Name"
      onChange={(e) => {
        onDropdownChange(e);
      }}
      title={'Select Stream Group'}
    />
  );
};

StreamGroupSelector.displayName = 'StreamGroupSelector';
