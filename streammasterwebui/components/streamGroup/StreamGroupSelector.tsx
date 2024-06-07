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
    return (
      <div className="sm-epg-selector">
        <div className="text-container">{selectedStreamGroup?.Name ?? 'Select Stream Group'}</div>
      </div>
    );
  }, [selectedStreamGroup?.Name]);

  return (
    <SMDropDown
      buttonDarkBackground
      buttonTemplate={buttonTemplate}
      data={data}
      dataKey="Id"
      isLoading={isLoading}
      itemTemplate={(option: StreamGroupDto) => (
        <div className="sm-epg-selector">
          <div className="text-container pl-1">{option?.Name}</div>
        </div>
      )}
      filter
      filterBy="Name"
      onChange={(e) => {
        onDropdownChange(e);
      }}
      title={'Select Stream Group'}
      contentWidthSize="2"
    />
  );
};

StreamGroupSelector.displayName = 'StreamGroupSelector';
