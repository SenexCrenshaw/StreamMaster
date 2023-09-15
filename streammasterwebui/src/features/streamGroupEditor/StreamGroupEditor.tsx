import { BlockUI } from "primereact/blockui";
import { memo } from "react";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import { StreamGroupEditorIcon } from "../../common/icons";
import StreamGroupDataSelector from "./StreamGroupDataSelector";
import StreamGroupSelectedVideoStreamDataSelector from "./StreamGroupSelectedVideoStreamDataSelector";
import StreamGroupVideoStreamDataSelector from "./StreamGroupVideoStreamDataSelector";

const StreamGroupEditor = () => {
  const id = 'streamgroupeditor'
  const { selectedStreamGroup } = useSelectedStreamGroup(id);

  return (
    <div className="streamGroupEditor">
      <div className="grid grid-nogutter flex justify-content-between align-items-center">

        <div className="flex w-full text-left font-bold text-white-500 surface-overlay justify-content-start align-items-center">
          <StreamGroupEditorIcon className='p-0 mr-1' />
          {StreamGroupEditor.displayName?.toUpperCase()}
        </div >
        <div className="flex col-12 mt-1 m-0 p-0" >

          <div className='col-3 m-0 p-0 pr-1' >
            <StreamGroupDataSelector id={id} />
          </div>

          <div className="col-9 m-0 p-0 pl-1">
            <BlockUI blocked={selectedStreamGroup === undefined || selectedStreamGroup.id === undefined || selectedStreamGroup.id <= 1 || selectedStreamGroup.isReadOnly}>
              <div className='grid grid-nogutter flex flex-wrap justify-content-between h-full col-12 p-0'>
                <div className='col-6'>
                  <StreamGroupVideoStreamDataSelector id={id} />
                </div>
                <div className='col-6'>
                  <StreamGroupSelectedVideoStreamDataSelector id={id} />
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

