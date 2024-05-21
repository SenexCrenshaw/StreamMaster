import { ChannelGroupDto, SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback } from 'react';
import { Dropdown, DropdownChangeEvent } from 'primereact/dropdown';
import { isEmptyObject } from '@lib/common/common';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';

interface SMChannelGroupEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  const { data } = useGetChannelGroups();

  const updateSMChanneGroup = useCallback(
    async (newGroup: string) => {
      console.log('newGroup', newGroup);

      if (isEmptyObject(smChannelDto)) {
        return;
      }
      const request: SetSMChannelGroupRequest = {
        Group: newGroup,
        SMChannelId: smChannelDto.Id
      };

      await SetSMChannelGroup(request);
    },
    [smChannelDto]
  );

  const itemTemplate = (option: ChannelGroupDto) => {
    if (option === undefined) {
      return null;
    }

    return <span>{option.Name}</span>;
  };
  return (
    <Dropdown
      className="w-full"
      filter
      itemTemplate={itemTemplate}
      showClear={false}
      clearIcon="pi pi-filter-slash"
      filterBy="Name"
      onChange={(e: DropdownChangeEvent) => {
        updateSMChanneGroup(e.value);
      }}
      options={data}
      optionLabel="Name"
      optionValue="Name"
      placeholder="Group"
      value={smChannelDto.Group}
    />
  );
};

export default memo(SMChannelGroupEditor);
