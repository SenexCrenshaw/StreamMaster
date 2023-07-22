import React from "react";
import type * as StreamMasterApi from '../../store/iptvApi';
import StreamDataSelector from '../../components/VideoStreamDataSelector';
import PlayListDataSelector from '../../components/PlayListDataSelector';
import { PlayListEditorIcon } from '../../common/icons';

const PlayListEditor = (props: PlayListEditorProps) => {
  const id = props.id ?? "playlisteditor";

  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);

  const onsetSelectedChannelGroups = React.useCallback((selectedData: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => {
    if (Array.isArray(selectedData)) {
      setSelectedChannelGroups(selectedData);
    } else {
      setSelectedChannelGroups([selectedData]);
    }

  }, [setSelectedChannelGroups]);


  return (
    <div className="playListEditor">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">
        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <PlayListEditorIcon className='p-0 mr-1' />
          {PlayListEditor.displayName?.toUpperCase()}
        </div >

        <div className="flex col-12 mt-1 m-0 p-0" >
          <div className='col-4 m-0 p-0 pr-1' >
            <PlayListDataSelector
              id={id}
              onSelectionChange={(e) => onsetSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[])}
            />
          </div>
          <div className="col-8 m-0 p-0">
            <StreamDataSelector
              groups={selectedChannelGroups}
              id='playlistTarget'
            />
          </div>
        </div >
      </div >

    </div>
  );
};

PlayListEditor.displayName = 'Playlist Editor';
PlayListEditor.defaultProps = {
  id: 'playlistEditor',
};

export type PlayListEditorProps = {
  /**
* The unique identifier of the component.
*/
  id?: string;
};

export default React.memo(PlayListEditor);
