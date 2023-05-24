/* eslint-disable @typescript-eslint/no-unused-vars */

import type * as StreamMasterApi from '../../store/iptvApi';
import React from 'react';
import StreamGroupDataSelector from '../../components/StreamGroupDataSelector';

import { StreamGroupEditorIcon } from '../../common/icons';
import PlayListDataSelector from '../../components/PlayListDataSelector';
import PlayListDataSelectorPicker from '../../components/PlayListDataSelectorPicker';

import { BlockUI } from 'primereact/blockui';

const StreamGroupEditor = () => {

  const [selectedStreamGroup, setSelectedStreamGroup] = React.useState<StreamMasterApi.StreamGroupDto>();

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
            <BlockUI blocked={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.id == 0}>
              <PlayListDataSelectorPicker
                id='streamgroupeditor-ds-streams'
                streamGroup={selectedStreamGroup}
              />
            </BlockUI>
          </div>

        </div>
      </div >
    </div >
  );
};

StreamGroupEditor.displayName = 'Stream Group Editor';
export default React.memo(StreamGroupEditor);
