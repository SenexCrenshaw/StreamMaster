
import React from "react";
import type * as StreamMasterApi from '../store/iptvApi';

import M3UFilesEditor from "./M3UFilesEditor";
import { M3UPlaylistEditorIcon } from "../common/icons";
import StreamDataSelector from "./VideoStreamDataSelector";


const M3UPlaylistEditor = () => {
  const [m3uFileDto, setM3uFileDto] = React.useState<StreamMasterApi.M3UFilesDto>({} as StreamMasterApi.M3UFilesDto);

  return (
    <div className="grid grid-nogutter flex justify-content-between align-items-center">
      <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
        <M3UPlaylistEditorIcon className='p-0 mr-1' />
        {M3UPlaylistEditor.displayName?.toUpperCase()}
      </div >
      <div className="flex col-12 mt-1 m-0 p-0" >
        <div className='col-5 m-0 p-0 pr-1' >
          <M3UFilesEditor
            onClick={(data: StreamMasterApi.M3UFilesDto) => { setM3uFileDto(data) }}
          />
        </div>

        <div className="col-7 m-0 p-0">
          <StreamDataSelector
            id='M3UPlaylist'
            m3uFileId={m3uFileDto.id}
          />
        </div>
      </div >
    </div >

  );
};


M3UPlaylistEditor.displayName = 'M3UPlaylistEditor';
M3UPlaylistEditor.defaultProps = {

};

export type M3UPlaylistEditorProps = {
  data: StreamMasterApi.VideoStreamDto;

};

export default React.memo(M3UPlaylistEditor);
