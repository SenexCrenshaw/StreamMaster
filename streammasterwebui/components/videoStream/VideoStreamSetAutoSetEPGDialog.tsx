import { VideoStreamsAutoSetEpgApiArg, VideoStreamsAutoSetEpgFromParametersApiArg, type VideoStreamDto } from '@lib/iptvApi';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';
import { AutoSetEpg, AutoSetEpgFromParameters } from '@lib/smAPI/VideoStreams/VideoStreamsMutateAPI';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import BookButton from '../buttons/BookButton';

type VideoStreamSetAutoSetEPGDialogProps = {
  readonly id: string;
  readonly iconFilled?: boolean | undefined;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly values?: VideoStreamDto[];
};

const VideoStreamSetAutoSetEPGDialog = ({ id, iconFilled = false, onClose, skipOverLayer, values }: VideoStreamSetAutoSetEPGDialogProps) => {
  const { selectedVideoStreams } = useSelectedVideoStreams(id);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [selectVideoStreamsInternal, setSelectVideoStreamsInternal] = useState<VideoStreamDto[] | undefined>(undefined);

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

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  }, [onClose]);

  const getTotalCount = useMemo(() => {
    return selectVideoStreamsInternal?.length ?? 0;
  }, [selectVideoStreamsInternal]);

  const onAutoSetEpg = useCallback(async () => {
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

      const toSendAll = {} as VideoStreamsAutoSetEpgFromParametersApiArg;

      toSendAll.parameters = queryFilter;
      // toSendAll.parameters.pageSize = getTotalCount;

      await AutoSetEpgFromParameters(toSendAll)
        .then(() => {
          setInfoMessage('Auto Set EPG Ran');
        })
        .catch((error) => {
          setInfoMessage('Auto Set EPG Error: ' + error.message);
        });

      return;
    }

    if (selectVideoStreamsInternal.length === 0) {
      ReturnToParent();

      return;
    }

    const toSend = {} as VideoStreamsAutoSetEpgApiArg;

    toSend.ids = selectVideoStreamsInternal.map((value) => value.id) ?? [];

    await AutoSetEpg(toSend)
      .then(() => {
        setInfoMessage('Auto Set EPG Ran');
      })
      .catch((error) => {
        setInfoMessage('Auto Set EPG Error: ' + error.message);
      });
  }, [ReturnToParent, getTotalCount, queryFilter, selectAll, selectVideoStreamsInternal]);

  if (skipOverLayer === true) {
    return <BookButton iconFilled={iconFilled} onClick={async () => await onAutoSetEpg()} tooltip="Auto Set EPG" />;
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Auto Set EPG?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className="flex justify-content-center w-full align-items-center h-full">
          <BookButton disabled={getTotalCount === 0 && !selectAll} label="Auto Set EPG" onClick={async () => await onAutoSetEpg()} />
        </div>
      </InfoMessageOverLayDialog>
      <BookButton disabled={getTotalCount === 0 && !selectAll} iconFilled={iconFilled} onClick={() => setShowOverlay(true)} tooltip="Auto Set EPG" />
    </>
  );
};

export default memo(VideoStreamSetAutoSetEPGDialog);
