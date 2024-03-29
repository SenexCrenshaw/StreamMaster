import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedVideoStreams } from '@lib/redux/slices/useSelectedVideoStreams';
import { memo, useEffect, useMemo, useState } from 'react';
import OKButton from '../buttons/OKButton';
import XButton from '../buttons/XButton';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';

interface VideoStreamDeleteDialogProperties {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: () => void;
  readonly skipOverLayer?: boolean;
  readonly values?: VideoStreamDto[];
}

const VideoStreamDeleteDialog = ({ iconFilled, id, onClose, skipOverLayer, values }: VideoStreamDeleteDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [block, setBlock] = useState<boolean>(false);

  const [videoStreamsDeleteAllVideoStreamsFromParametersMutation] = useVideoStreamsDeleteAllVideoStreamsFromParametersMutation();
  const [videoStreamsDeleteVideoStreamMutation] = useVideoStreamsDeleteVideoStreamMutation();

  const [selectVideoStreamsInternal, setSelectVideoStreamsInternal] = useState<VideoStreamDto[] | undefined>();

  const { selectedVideoStreams } = useSelectedVideoStreams(id);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  useEffect(() => {
    if (values && selectVideoStreamsInternal !== values) {
      setSelectVideoStreamsInternal(values);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [values]); // This is correct

  useEffect(() => {
    if (!values) {
      setSelectVideoStreamsInternal(selectedVideoStreams);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [values]);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    onClose?.();
  };

  const deleteVideoStream = async () => {
    setBlock(true);

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();

        return;
      }

      const toSendAll = {} as VideoStreamsDeleteAllVideoStreamsFromParametersApiArg;

      toSendAll.parameters = queryFilter;

      await videoStreamsDeleteAllVideoStreamsFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Set Stream Visibility Successfully');
        })
        .catch((error) => {
          setInfoMessage(`Set Stream Visibility Error: ${error.message}`);
        });

      return;
    }

    if (!selectVideoStreamsInternal || selectVideoStreamsInternal?.length === 0) {
      ReturnToParent();

      return;
    }

    const promises = [];

    for (const stream of selectVideoStreamsInternal) {
      const data = {} as VideoStreamsDeleteVideoStreamApiArg;

      data.id = stream.id;
      promises.push(
        videoStreamsDeleteVideoStreamMutation(data)
          .then(() => {})
          .catch(() => {})
      );
    }

    const p = Promise.all(promises);

    await p
      .then(() => {
        setInfoMessage('Delete Stream Successful');
      })
      .catch((error) => {
        setInfoMessage(`Delete Stream Error: ${error.message}`);
      });
  };

  const isFirstDisabled = useMemo(() => {
    if (!selectVideoStreamsInternal || selectVideoStreamsInternal?.length === 0) {
      return true;
    }

    return !selectVideoStreamsInternal[0].isUserCreated;
  }, [selectVideoStreamsInternal]);

  const getTotalCount = useMemo(() => {
    const count = selectVideoStreamsInternal?.length ?? 0;

    return count;
  }, [selectVideoStreamsInternal?.length]);

  if (skipOverLayer) {
    return <XButton disabled={isFirstDisabled} onClick={async () => await deleteVideoStream()} tooltip="Delete User Created Stream" />;
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Delete?"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className="m-0 p-0 w-full">
          {/* <div className="m-3 border-1 border-red-500 w-full"> */}
          <div className="card flex mt-3 flex-wrap gap-2 justify-content-center ">
            <OKButton onClick={async () => await deleteVideoStream()} />
          </div>
          {/* </div> */}
        </div>
      </InfoMessageOverLayDialog>

      <XButton
        disabled={(isFirstDisabled || getTotalCount === 0) && !selectAll}
        iconFilled={iconFilled}
        onClick={() => setShowOverlay(true)}
        tooltip="Delete User Created Stream"
      />
    </>
  );
};

VideoStreamDeleteDialog.displayName = 'VideoStreamDeleteDialog';

export default memo(VideoStreamDeleteDialog);
