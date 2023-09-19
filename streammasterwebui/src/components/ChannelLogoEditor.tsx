import { memo } from "react";
import { UpdateVideoStream } from "../smAPI/VideoStreams/VideoStreamsMutateAPI";
import { type UpdateVideoStreamRequest, type VideoStreamDto } from "../store/iptvApi";
import IconSelector from "./selectors/IconSelector";

const ChannelLogoEditor = (props: StreamDataSelectorProps) => {

  const onUpdateVideoStream = async (Logo: string) => {
    if (props.data.id === '') {
      return;
    }

    const data = {} as UpdateVideoStreamRequest;

    data.id = props.data.id;

    if (Logo && Logo !== '' && props.data.user_Tvg_logo !== Logo) {
      data.tvg_logo = Logo;
    }

    await UpdateVideoStream(data)
      .then(() => {

      }).catch((e) => {
        console.error(e);
      });

  };

  return (
    <IconSelector
      className="p-inputtext-sm"
      enableEditMode={props.enableEditMode}
      onChange={
        async (e: string) => {
          await onUpdateVideoStream(e);
        }
      }
      value={props.data.user_Tvg_logo}
    />
  );
};

ChannelLogoEditor.displayName = 'Logo Editor';
ChannelLogoEditor.defaultProps = {
  enableEditMode: true
};

export type StreamDataSelectorProps = {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
};

export default memo(ChannelLogoEditor);
