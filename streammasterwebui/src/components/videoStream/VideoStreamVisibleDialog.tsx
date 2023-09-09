import { memo, useCallback, useMemo, useState } from "react";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useVideoStreamsUpdateAllVideoStreamsFromParametersMutation, useVideoStreamsUpdateVideoStreamsMutation, type UpdateVideoStreamRequest, type UpdateVideoStreamsRequest, type VideoStreamDto, type VideoStreamsUpdateAllVideoStreamsFromParametersApiArg } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import VisibleButton from "../buttons/VisibleButton";

type VideoStreamVisibleDialogProps = {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: (() => void);
  readonly skipOverLayer?: boolean;
  readonly values?: VideoStreamDto[] | undefined;
};

const VideoStreamVisibleDialog = ({
  id,
  iconFilled,
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
          setInfoMessage('Toggle Stream Visibility Successfully');
        }
        ).catch((error) => {
          setInfoMessage('Toggle Stream Visibility Error: ' + error.message);
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
        label='Toggle Visibility'
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
        header='Toggle Visibility?'
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={2}
        show={showOverlay}
      >

        <div className="flex justify-content-center w-full">
          <VisibleButton disabled={getTotalCount === 0 && !selectAll} onClick={async () => await onVisiblesClick()} />
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
