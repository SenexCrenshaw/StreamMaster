import { useState, useCallback, useMemo, memo } from "react";
import { type VideoStreamsUpdateAllVideoStreamsFromParametersApiArg } from "../../store/iptvApi";
import { type VideoStreamDto, type UpdateVideoStreamsRequest, type UpdateVideoStreamRequest, useVideoStreamsUpdateAllVideoStreamsFromParametersMutation } from "../../store/iptvApi";
import { useVideoStreamsUpdateVideoStreamsMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import VisibleButton from "../buttons/VisibleButton";
import OKButton from "../buttons/OKButton";
import { useSelectAll } from "../../app/slices/useSelectAll";

type VideoStreamVisibleDialogProps = {
  iconFilled?: boolean;
  id: string;
  onClose?: (() => void);
  skipOverLayer?: boolean;
  values?: VideoStreamDto[] | undefined;
};

const VideoStreamVisibleDialog = ({
  iconFilled,
  id,
  onClose,
  skipOverLayer,
  values,
}: VideoStreamVisibleDialogProps) => {

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [selectedVideoStreams, setSelectedVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const [videoStreamsUpdateVideoStreams] = useVideoStreamsUpdateVideoStreamsMutation();
  const [videoStreamsUpdateAllVideoStreamsFromParametersMutation] = useVideoStreamsUpdateAllVideoStreamsFromParametersMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  useMemo(() => {

    if (values !== null) {
      setSelectedVideoStreams(values ?? []);
    }

  }, [values]);


  const getTotalCount = useMemo(() => {

    return selectedVideoStreams.length;

  }, [selectedVideoStreams.length]);


  const onVisiblesClick = useCallback(async () => {
    setBlock(true);

    if (getTotalCount !== 1 && selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();
        return;
      }

      const toSendAll = {} as VideoStreamsUpdateAllVideoStreamsFromParametersApiArg;

      toSendAll.parameters = queryFilter;
      // toSendAll.parameters.pageSize = getTotalCount;

      toSendAll.request = {
        toggleVisibility: true
      } as UpdateVideoStreamRequest;

      videoStreamsUpdateAllVideoStreamsFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Set Stream Visibility Successfully');
        }
        ).catch((error) => {
          setInfoMessage('Set Stream Visibility Error: ' + error.message);
        });
      return;
    }

    if (selectedVideoStreams.length === 0) {
      ReturnToParent();
      return;
    }

    const toSend = {} as UpdateVideoStreamsRequest;

    toSend.videoStreamUpdates = selectedVideoStreams.map((a) => {
      return {
        id: a.id,
        toggleVisibility: true
      } as UpdateVideoStreamRequest;
    });


    videoStreamsUpdateVideoStreams(toSend)
      .then(() => {
        setInfoMessage('Set Stream Visibility Successfully');
      }
      ).catch((error) => {
        setInfoMessage('Set Stream Visibility Error: ' + error.message);
      });

  }, [selectedVideoStreams, getTotalCount, selectAll, videoStreamsUpdateVideoStreams, ReturnToParent, queryFilter, videoStreamsUpdateAllVideoStreamsFromParametersMutation]);


  if (skipOverLayer === true) {
    return (

      <VisibleButton
        disabled={getTotalCount === 0}
        iconFilled={false}
        onClick={async () => await onVisiblesClick()}
        tooltip='Toggle Visibility'
      />

    );
  }



  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={`Toggle visibility for ${getTotalCount < 2 ? getTotalCount + ' Stream ?' : getTotalCount + ' Streams ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        show={showOverlay}
      >

        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <OKButton onClick={async () => await onVisiblesClick()} />
            </div>
          </div>
        </div>

      </InfoMessageOverLayDialog >
      <VisibleButton
        disabled={getTotalCount === 0 && !selectAll}
        iconFilled={iconFilled}
        onClick={() => setShowOverlay(true)}
        tooltip='Toggle Visibility'
      />
    </>
  );
}

VideoStreamVisibleDialog.displayName = 'VideoStreamVisibleDialog';

export default memo(VideoStreamVisibleDialog);
