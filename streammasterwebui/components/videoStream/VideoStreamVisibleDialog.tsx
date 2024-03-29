import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';

import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import VisibleButton from '../buttons/VisibleButton';

interface VideoStreamVisibleDialogProperties {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly values?: VideoStreamDto[];
}

const VideoStreamVisibleDialog = ({ id, iconFilled, onClose, skipOverLayer, values }: VideoStreamVisibleDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [selectVideoStreamsInternal, setSelectVideoStreamsInternal] = useState<VideoStreamDto[] | undefined>();

  const { selectedVideoStreams } = useSelectedVideoStreams(id);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  useEffect(() => {
    if (values) {
      setSelectVideoStreamsInternal(values);
    }
  }, [values]);

  useEffect(() => {
    if (!values) {
      setSelectVideoStreamsInternal(selectedVideoStreams);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedVideoStreams]);

  const getTotalCount = useMemo(() => selectVideoStreamsInternal?.length ?? 0, [selectVideoStreamsInternal]);

  const onVisiblesClick = useCallback(async () => {
    if (selectVideoStreamsInternal === undefined) {
      ReturnToParent();
      return;
    }

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

      // await UpdateAllVideoStreamsFromParameters(toSendAll)
      //   .then(() => {
      //     setInfoMessage('Toggle Stream Visibility Successfully');
      //   })
      //   .catch((error) => {
      //     setInfoMessage(`Toggle Stream Visibility Error: ${error.message}`);
      //   });

      return;
    }

    if (selectVideoStreamsInternal.length === 0) {
      ReturnToParent();

      return;
    }

    const toSend = {} as UpdateVideoStreamsRequest;

    toSend.videoStreamUpdates = selectVideoStreamsInternal.map(
      (a) =>
        ({
          id: a.id,
          toggleVisibility: true
        } as UpdateVideoStreamRequest)
    );

    // await UpdateVideoStreams(toSend)
    //   .then(() => {
    //     setInfoMessage('Set Stream Visibility Successfully');
    //   })
    //   .catch((error) => {
    //     setInfoMessage(`Set Stream Visibility Error: ${error.message}`);
    //   });
  }, [selectVideoStreamsInternal, getTotalCount, selectAll, ReturnToParent, queryFilter]);

  if (skipOverLayer === true) {
    return (
      <VisibleButton
        disabled={getTotalCount === 0}
        iconFilled={false}
        label="Toggle Visibility"
        onClick={async () => await onVisiblesClick()}
        tooltip="Toggle Visibility"
      />
    );
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Toggle Visibility?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full align-items-center h-full">
          <VisibleButton disabled={getTotalCount === 0 && !selectAll} label="Toggle Visibility" onClick={async () => await onVisiblesClick()} />
        </div>
      </InfoMessageOverLayDialog>
      <VisibleButton disabled={getTotalCount === 0 && !selectAll} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Toggle Visibility" />
    </>
  );
};

VideoStreamVisibleDialog.displayName = 'VideoStreamVisibleDialog';

export default memo(VideoStreamVisibleDialog);
