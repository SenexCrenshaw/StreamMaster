import { useLocalStorage } from "primereact/hooks";
import { useCallback, memo } from "react";
import { PlayListEditorIcon } from "../../common/icons";
import PlayListDataSelector from "../../components/dataSelectors/PlayListDataSelector";
import VideoStreamDataSelector from "../../components/dataSelectors/VideoStreamDataSelector";
import { type ChannelGroupDto } from "../../store/iptvApi";

const PlayListEditor = (props: PlayListEditorProps) => {
  const id = props.id ?? "playlisteditor";

  const [selectedChannelGroups, setSelectedChannelGroups] = useLocalStorage<string[]>([] as string[], props.id + '-selectedChannelGroups');

  const onsetSelectedChannelGroups = useCallback((selectedData: ChannelGroupDto | ChannelGroupDto[]) => {
    if (Array.isArray(selectedData)) {
      setSelectedChannelGroups(selectedData.map(a => a.name));
    } else {
      setSelectedChannelGroups([selectedData.name]);
    }

    // console.debug('onsetSelectedChannelGroups', selectedData);
  }, [setSelectedChannelGroups]);

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
            <PlayListDataSelector
              id={id}
              onSelectionChange={(e) => onsetSelectedChannelGroups(e as ChannelGroupDto[])}
            />
          </div>
          <div className="col-8 m-0 p-0">
            <VideoStreamDataSelector
              channelGroupNames={selectedChannelGroups}
              id={id}
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

export default memo(PlayListEditor);
