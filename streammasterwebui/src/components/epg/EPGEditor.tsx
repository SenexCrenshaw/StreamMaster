import { memo } from "react";
import { useVideoStreamsUpdateVideoStreamMutation, type UpdateVideoStreamRequest, type VideoStreamDto } from "../../store/iptvApi";
import EPGSelector from "../selectors/EPGSelector";

const EPGEditor = (props: EPGEditorProps) => {
  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  const onUpdateVideoStream = async (epg: string) => {
    if (props.data.id === '') {
      return;
    }

    const data = {} as UpdateVideoStreamRequest;

    data.id = props.data.id;

    if (epg && epg !== '' && props.data.user_Tvg_ID !== epg) {
      data.tvg_ID = epg;
    }

    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {

      }).catch((e: unknown) => {
        console.error(e);
      });

  };

  return (
    <div className="flex w-full">
      <EPGSelector
        enableEditMode={props.enableEditMode}
        onChange={
          async (e: string) => {
            await onUpdateVideoStream(e);
          }
        }
        value={props.data.user_Tvg_ID}
      />
    </div>
  );
};

EPGEditor.displayName = 'EPG Editor';
EPGEditor.defaultProps = {
  enableEditMode: true
};

type EPGEditorProps = {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
};

export default memo(EPGEditor);
