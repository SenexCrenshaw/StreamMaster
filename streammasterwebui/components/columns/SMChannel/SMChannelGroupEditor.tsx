import { isEmptyObject } from '@lib/common/common';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { ChannelGroupDto, SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { ReactNode, memo, useCallback, useMemo, useRef } from 'react';

import SMDropDown, { SMDropDownRef } from '@components/sm/SMDropDown';

interface SMChannelGroupEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ darkBackGround, smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  // const dataKey = 'SMChannelGroup-Groups';
  const smDropownRef = useRef<SMDropDownRef>(null);
  const { data } = useGetChannelGroups();
  // const { selectedItems } = useSelectedItems<ChannelGroupDto>(dataKey);

  const handleOnChange = useCallback(
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

  const buttonTemplate = useMemo((): ReactNode => {
    if (!smChannelDto) {
      return <div className="text-xs text-container text-white-alpha-40">None</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{smChannelDto.Group}</div>
      </div>
    );
  }, [smChannelDto]);

  // const headerTemplate = useMemo((): ReactNode => {
  //   if (selectedItems && selectedItems.length > 0) {
  //     const epgNames = selectedItems.slice(0, 2).map((x) => x.Name);
  //     const suffix = selectedItems.length > 2 ? ',...' : '';
  //     return <div className="px-4 w-10rem flex align-content-center justify-content-center min-w-10rem">{epgNames.join(', ') + suffix}</div>;
  //   }
  //   return <div className="px-4 w-10rem" style={{ minWidth: '10rem' }} />;
  // }, [selectedItems]);
  // Logger.debug('SMChannelGroupEditor', 'smChannelDto', smChannelDto, data);
  return (
    <SMDropDown
      buttonLabel="GROUP"
      buttonDarkBackground={darkBackGround}
      buttonTemplate={buttonTemplate}
      data={data}
      dataKey="Group"
      filter
      filterBy="Name"
      itemTemplate={itemTemplate}
      onChange={(e) => {
        handleOnChange(e.Name);
      }}
      ref={smDropownRef}
      title="GROUP"
      value={smChannelDto}
      optionValue="Name"
      widthSize="2"
    />
  );
};

export default memo(SMChannelGroupEditor);
