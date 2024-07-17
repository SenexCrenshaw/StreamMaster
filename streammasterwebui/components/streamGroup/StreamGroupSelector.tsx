import SMDropDown from '@components/sm/SMDropDown';
import useGetStreamGroups from '@lib/smAPI/StreamGroups/useGetStreamGroups';
import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { useEffect, useMemo, useState } from 'react';
import StreamGroupCreateDialog from './StreamGroupCreateDialog';

interface StreamGroupSelectorProperties {
  readonly buttonDisabled?: boolean;
  readonly label?: string;
  readonly selectedStreamGroup?: StreamGroupDto;
  readonly value?: string;
  readonly zIndex: number;
  readonly onChange: (value: StreamGroupDto) => void;
}

export const StreamGroupSelector = ({ buttonDisabled, label, onChange, selectedStreamGroup, value, zIndex }: StreamGroupSelectorProperties) => {
  const { data, isLoading } = useGetStreamGroups();
  const [intSelectedStreamGroup, setIntSelectedStreamGroup] = useState<StreamGroupDto>();

  useEffect(() => {
    if (value !== undefined && value !== '' && data && data.length > 0) {
      var found = data.find((x) => x.Name === value);
      if (found) {
        setIntSelectedStreamGroup(found);
      }
      return;
    }
    if (selectedStreamGroup) {
      setIntSelectedStreamGroup(selectedStreamGroup);
    }
  }, [data, selectedStreamGroup, value]);

  const onDropdownChange = (sg: StreamGroupDto) => {
    if (!sg) return;
    if (intSelectedStreamGroup && sg.Id === intSelectedStreamGroup.Id) return;
    onChange?.(sg);
  };

  const buttonTemplate = useMemo(() => {
    return <div className="text-container ">{intSelectedStreamGroup?.Name ?? 'Select Stream Group'}</div>;
  }, [intSelectedStreamGroup?.Name]);

  const headerRightTemplate = useMemo(() => <StreamGroupCreateDialog modal zIndex={12} />, []);

  return (
    <SMDropDown
      buttonDarkBackground
      buttonDisabled={buttonDisabled}
      buttonIsLoading={isLoading}
      buttonLarge
      buttonTemplate={buttonTemplate}
      contentWidthSize="2"
      data={data}
      dataKey="Id"
      filter
      filterBy="Name"
      header={headerRightTemplate}
      info=""
      itemTemplate={(option: StreamGroupDto) => (
        <div className="sm-epg-selector2">
          <div className="text-container pl-1 sm-w-15rem">{option?.Name}</div>
        </div>
      )}
      label={label}
      modal
      onChange={(e) => {
        onDropdownChange(e);
      }}
      title="Stream Group"
      value={intSelectedStreamGroup}
      zIndex={zIndex}
    />
  );
};

StreamGroupSelector.displayName = 'StreamGroupSelector';
