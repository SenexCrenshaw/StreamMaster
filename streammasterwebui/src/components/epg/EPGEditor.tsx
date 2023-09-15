/* eslint-disable @typescript-eslint/no-unused-vars */
import { memo } from "react";
import { UpdateVideoStream } from "../../smAPI/VideoStreams/VideoStreamsMutateAPI";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import EPGSelector from "../selectors/EPGSelector";

type EPGEditorProps = {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
  readonly id?: string;
};

const EPGEditor = ({ data, enableEditMode, id }: EPGEditorProps) => {

  const onUpdateVideoStream = async (epg: string) => {
    if (data.id === '') {
      return;
    }

    const toSend = {} as UpdateVideoStreamRequest;

    toSend.id = data.id;

    if (epg && epg !== '' && data.user_Tvg_ID !== epg) {
      toSend.tvg_ID = epg;
    }


    await UpdateVideoStream(toSend)
      .then(() => {

      }).catch((e: unknown) => {
        console.error(e);

      });

  };

  return (
    <div className="flex w-full">
      <EPGSelector
        enableEditMode={enableEditMode}
        onChange={
          async (e: string) => {
            await onUpdateVideoStream(e);
          }
        }
        value={data.user_Tvg_ID}
      />
    </div>
  );
};




export default memo(EPGEditor);
