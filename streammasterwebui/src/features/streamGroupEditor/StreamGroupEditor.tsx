/* eslint-disable @typescript-eslint/no-unused-vars */
import type * as StreamMasterApi from '../../store/iptvApi';
import React from 'react';
import StreamGroupDataSelector from '../../components/StreamGroupDataSelector';

import { StreamGroupEditorIcon } from '../../common/icons';
import PlayListDataSelectorPicker from '../../components/PlayListDataSelectorPicker';

import { BlockUI } from 'primereact/blockui';
import { useLocalStorage } from 'primereact/hooks';
import PlayListDataSelector from '../../components/PlayListDataSelector';
import StreamGroupVideoStreamDataSelector from '../../components/StreamGroupVideoStreamDataSelector';
import VideoStreamDataSelector from '../../components/VideoStreamDataSelector';

const StreamGroupEditor = () => {
  const id = 'streamgroupeditor;'
  const [selectedStreamGroup, setSelectedStreamGroup] = useLocalStorage<StreamMasterApi.StreamGroupDto | undefined>(undefined, id + '-selectedstreamgroup');
  const [selectedChannelGroups, setSelectedChannelGroups] = useLocalStorage<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[], id + '-selectedChannelGroups');

  const onsetSelectedChannelGroups = React.useCallback((selectedData: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => {
    if (Array.isArray(selectedData)) {
      setSelectedChannelGroups(selectedData);
    } else {
      setSelectedChannelGroups([selectedData]);
    }

    console.debug('onsetSelectedChannelGroups');
  }, [setSelectedChannelGroups]);


  return (
    <div className="streamGroupEditor">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">

        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <StreamGroupEditorIcon className='p-0 mr-1' />
          {StreamGroupEditor.displayName?.toUpperCase()}
        </div >
        <div className="flex col-12 mt-1 m-0 p-0" >

          <div className='col-3 m-0 p-0 pr-1' >
            <StreamGroupDataSelector
              id="streamgroupeditor-ds-source"
              onSelectionChange={(e) => {
                const sg = e as StreamMasterApi.StreamGroupDto;
                if (sg.id !== undefined) {
                  setSelectedStreamGroup(sg);
                } else {
                  setSelectedStreamGroup(undefined);
                }

              }}
            />
          </div>

          <div className="col-9 m-0 p-0 pl-1">
            <BlockUI blocked={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.id === 0}>
              {/* <PlayListDataSelectorPicker
                enableState={false}
                id='streamgroupeditor-ds-streams'
                showHidden={false}
                streamGroup={selectedStreamGroup}
              /> */}
              <div className='grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0'>
                <div className='col-6'>
                  <VideoStreamDataSelector
                    channelGroups={selectedChannelGroups}
                    enableEditMode={false}
                    id={id}
                    showBrief
                  />
                </div>
                <div className='col-6'>
                  <StreamGroupVideoStreamDataSelector
                    channelGroups={selectedChannelGroups}
                    enableEditMode={false}
                    id={id}
                    showBrief
                  />
                </div>
              </div>
            </BlockUI>
          </div>

        </div>
      </div >
    </div >
  );
};

StreamGroupEditor.displayName = 'Stream Group Editor';
export default React.memo(StreamGroupEditor);

