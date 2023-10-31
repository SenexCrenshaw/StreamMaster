import { M3UPlaylistEditorIcon } from '@lib/common/icons';
import { type VideoStreamDto } from '@lib/iptvApi';
import { memo } from 'react';
import M3UFilesEditor from './M3UFilesEditor';

const M3UPlaylistEditor = () => (
  // const [m3uFileDto, setM3uFileDto] = useState<M3UFileDto>({} as M3UFileDto);

  <div className="grid grid-nogutter flex justify-content-between align-items-center">
    <div className="flex w-full text-left ml-1 font-bold text-white-500 surface-overlay justify-content-start align-items-center">
      <M3UPlaylistEditorIcon className="p-0 mr-1" />
      {M3UPlaylistEditor.displayName?.toUpperCase()}
    </div>
    <div className="flex col-12 mt-1 m-0 p-0">
      {/* <div className='col-5 m-0 p-0 pr-1' > */}
      <M3UFilesEditor />
      {/* </div> */}

      {/* <div className="col-7 m-0 p-0">
          <StreamDataSelector
            id='M3UPlaylist'
            m3uFileId={m3uFileDto.id}
          />
        </div> */}
    </div>
  </div>
);
M3UPlaylistEditor.displayName = 'M3UPlaylistEditor';

export interface M3UPlaylistEditorProperties {
  data: VideoStreamDto;
}

export default memo(M3UPlaylistEditor);
