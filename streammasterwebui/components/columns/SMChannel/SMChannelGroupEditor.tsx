import { ChannelGroupDto, SMChannelDto, SetSMChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import { ReactNode, Suspense, lazy, memo, useCallback, useMemo } from 'react';
import { isEmptyObject } from '@lib/common/common';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { SetSMChannelGroup } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMOverlay } from '@components/sm/SMOverlay';

import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

const SMScroller = lazy(() => import('@components/sm/SMScroller'));

interface SMChannelGroupEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
}

const SMChannelGroupEditor = ({ darkBackGround, smChannelDto, onChange }: SMChannelGroupEditorProperties) => {
  const dataKey = 'SMChannelGroup-Groups';
  const { data } = useGetChannelGroups();
  const { selectedItems } = useSelectedItems<ChannelGroupDto>(dataKey);

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

  const headerTemplate = useMemo((): ReactNode => {
    if (selectedItems && selectedItems.length > 0) {
      const epgNames = selectedItems.slice(0, 2).map((x) => x.Name);
      const suffix = selectedItems.length > 2 ? ',...' : '';
      return <div className="px-4 w-10rem flex align-content-center justify-content-center min-w-10rem">{epgNames.join(', ') + suffix}</div>;
    }
    return <div className="px-4 w-10rem" style={{ minWidth: '10rem' }} />;
  }, [selectedItems]);

  return (
    <div className={darkBackGround ? 'sm-input-border-dark w-full' : 'w-full'}>
      <SMOverlay header={headerTemplate} title="GROUP" widthSize="2" icon="pi-chevron-down" buttonTemplate={buttonTemplate} buttonLabel="EPG">
        <div className="flex flex-row w-12 sm-card border-radius-left border-radius-right ">
          <Suspense>
            <div className="flex w-12">
              <SMScroller
                data={data}
                dataKey="Group"
                filter
                filterBy="Name"
                itemSize={26}
                itemTemplate={itemTemplate}
                onChange={(e) => {
                  updateSMChanneGroup(e.Name);
                }}
                scrollHeight={150}
                value={smChannelDto}
              />
            </div>
          </Suspense>
        </div>
      </SMOverlay>
    </div>
  );
};

export default memo(SMChannelGroupEditor);
