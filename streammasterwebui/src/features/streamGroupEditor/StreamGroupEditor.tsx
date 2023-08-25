/* eslint-disable @typescript-eslint/no-unused-vars */
import { BlockUI } from "primereact/blockui";
import { useLocalStorage } from "primereact/hooks";
import { useCallback, memo } from "react";
import { StreamGroupEditorIcon } from "../../common/icons";
import StreamGroupDataSelector from "../../components/dataSelectors/StreamGroupDataSelector";
import { type StreamGroupDto, type ChannelGroupDto } from "../../store/iptvApi";
import StreamGroupVideoStreamDataSelector from "../../components/dataSelectors/StreamGroupVideoStreamDataSelector";
import StreamGroupVideoStreamDataOutSelector from "../../components/dataSelectors/StreamGroupVideoStreamDataOutSelector";

const StreamGroupEditor = () => {
  const id = 'streamgroupeditor'
  const [selectedStreamGroup, setSelectedStreamGroup] = useLocalStorage<StreamGroupDto>({ id: 0 } as StreamGroupDto, id + '-selectedstreamgroup');

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
              onSelectionChange={(sg) => {
                if (sg !== undefined) {
                  setSelectedStreamGroup(sg);
                }
              }}
            />
          </div>

          <div className="col-9 m-0 p-0 pl-1">
            <BlockUI blocked={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.isReadOnly}>
              <div className='grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0'>
                <div className='col-6'>
                  <StreamGroupVideoStreamDataSelector
                    id={id}
                    streamGroup={selectedStreamGroup}
                  />
                </div>
                <div className='col-6'>
                  <StreamGroupVideoStreamDataOutSelector
                    id={id}
                    streamGroup={selectedStreamGroup}
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
export default memo(StreamGroupEditor);

