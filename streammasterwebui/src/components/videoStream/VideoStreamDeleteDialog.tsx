
import { memo, useMemo, useState } from "react";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useVideoStreamsDeleteAllVideoStreamsFromParametersMutation, useVideoStreamsDeleteVideoStreamMutation, type VideoStreamDto, type VideoStreamsDeleteAllVideoStreamsFromParametersApiArg, type VideoStreamsDeleteVideoStreamApiArg } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import DeleteButton from "../buttons/DeleteButton";
import OKButton from "../buttons/OKButton";

type VideoStreamDeleteDialogProps = {
  readonly iconFilled?: boolean;
  readonly id: string;
  readonly onClose?: (() => void);
  readonly skipOverLayer?: boolean;
  readonly values?: VideoStreamDto[] | undefined;
};

const VideoStreamDeleteDialog = ({
  iconFilled,
  id,
  onClose,
  skipOverLayer,
  values,
}: VideoStreamDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');

  const [block, setBlock] = useState<boolean>(false);

  const [videoStreamsDeleteAllVideoStreamsFromParametersMutation] = useVideoStreamsDeleteAllVideoStreamsFromParametersMutation();
  const [videoStreamsDeleteVideoStreamMutation] = useVideoStreamsDeleteVideoStreamMutation();

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

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
        }
        ).catch((error) => {
          setInfoMessage('Set Stream Visibility Error: ' + error.message);
        });

      return;
    }

    if ((!values || values?.length === 0)) {
      ReturnToParent();

      return;
    }

    const promises = [];

    for (const stream of values) {
      const data = {} as VideoStreamsDeleteVideoStreamApiArg;

      data.id = stream.id;
      promises.push(
        videoStreamsDeleteVideoStreamMutation(data).then(() => { }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {
      setInfoMessage('Delete Stream Successful');
    }).catch((error) => {
      setInfoMessage('Delete Stream Error: ' + error.message);
    });

  }

  const isFirstDisabled = useMemo(() => {
    if (!values || values?.length === 0) {
      return true;
    }

    return !values[0].isUserCreated;

  }, [values]);

  const getTotalCount = useMemo(() => {
    let count = values?.length ?? 0;

    return count;

  }, [values?.length]);


  if (skipOverLayer) {
    return (
      <DeleteButton disabled={isFirstDisabled} iconFilled={iconFilled} onClick={async () => await deleteVideoStream()} tooltip="Delete User Created Stream" />
    );
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header='Delete Streams?'
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={2}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <OKButton onClick={async () => await deleteVideoStream()} />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <DeleteButton
        disabled={isFirstDisabled || getTotalCount === 0 && !selectAll}
        iconFilled={iconFilled}
        onClick={() => setShowOverlay(true)}
        tooltip="Delete User Created Streams" />

    </>
  );
}

VideoStreamDeleteDialog.displayName = 'VideoStreamDeleteDialog';


export default memo(VideoStreamDeleteDialog);
