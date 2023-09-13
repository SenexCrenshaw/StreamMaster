import { memo } from "react";
import { PlayListEditorIcon } from "../../common/icons";

import { useSelectedChannelGroups } from "../../app/slices/useSelectedChannelGroups";
import ChannelGroupVideoStreamDataSelector from "./ChannelGroupVideoStreamDataSelector";
import PlayListDataSelector from "./PlayListDataSelector";

const PlayListEditor = () => {
  const id = "playlisteditor";
  const { selectedChannelGroups } = useSelectedChannelGroups(id);

  if (selectedChannelGroups === undefined) {
    return null;
  }

  return (
    <div className="playListEditor">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">
        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <PlayListEditorIcon className='p-0 mr-1' />
          {PlayListEditor.displayName?.toUpperCase()}
        </div >

        <div className="flex col-12 mt-1 m-0 p-0" >
          <div className='col-4 m-0 p-0 pr-1' >
            <PlayListDataSelector id={id} />
          </div>
          <div className="col-8 m-0 p-0">
            <ChannelGroupVideoStreamDataSelector id={id} />
          </div>
        </div >
      </div >

    </div>
  );
};

PlayListEditor.displayName = 'Playlist Editor';

export default memo(PlayListEditor);
