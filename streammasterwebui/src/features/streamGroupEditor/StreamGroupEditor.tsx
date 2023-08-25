/* eslint-disable @typescript-eslint/no-unused-vars */
import { BlockUI } from "primereact/blockui";
import { useLocalStorage } from "primereact/hooks";
import { useCallback, memo } from "react";
import { StreamGroupEditorIcon } from "../../common/icons";
import StreamGroupDataSelector from "../../components/streamGroup/StreamGroupDataSelector";
import VideoStreamDataSelector from "../../components/dataSelectors/VideoStreamDataSelector";
import { type StreamGroupDto, type ChannelGroupDto } from "../../store/iptvApi";

const StreamGroupEditor = () => {
  const id = 'streamgroupeditor'
  const [selectedStreamGroup, setSelectedStreamGroup] = useLocalStorage<StreamGroupDto | undefined>(undefined, id + '-selectedstreamgroup');
  const [selectedChannelGroups, setSelectedChannelGroups] = useLocalStorage<ChannelGroupDto[]>([] as ChannelGroupDto[], id + '-selectedChannelGroups');

  const onsetSelectedChannelGroups = useCallback((selectedData: ChannelGroupDto | ChannelGroupDto[]) => {
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
                const sg = e as StreamGroupDto;
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

                id='streamgroupeditor-ds-streams'
                showHidden={false}
                streamGroup={selectedStreamGroup}
              /> */}
              <div className='grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0'>
                <div className='col-6'>
                  <VideoStreamDataSelector
                    channelGroupNames={selectedChannelGroups.map(a => a.name)}
                    enableEditMode={false}
                    id={id}
                    showBrief
                  />
                </div>
                <div className='col-6'>
                  {/* <StreamGroupVideoStreamDataSelector
                    channelGroups={selectedChannelGroups}
                    enableEditMode={false}
                    id={id}
                    showBrief
                  /> */}
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
export default memo(StreamGroupEditor);

