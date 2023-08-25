
import { useState, useMemo, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type VideoStreamDto, type DeleteVideoStreamRequest } from "../../store/iptvApi";
import { DeleteVideoStream } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import { Button } from "primereact/button";
import OKButton from "../buttons/OKButton";
import DeleteButton from "../buttons/DeleteButton";

const VideoStreamDeleteDialog = (props: VideoStreamDeleteDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [selectedVideoStreams, setSelectedVideoStreams] = useState<VideoStreamDto[]>([] as VideoStreamDto[]);
  const [block, setBlock] = useState<boolean>(false);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
    props.onClose?.();
  };

  useMemo(() => {

    if (props.values !== null && props.values !== undefined) {
      setSelectedVideoStreams(props.values);
    }

  }, [props.values]);

  useMemo(() => {

    if (props.value !== null && props.value !== undefined) {
      setSelectedVideoStreams([props.value]);
    }

  }, [props.value]);

  const deleteVideoStream = async () => {
    setBlock(true);
    if (selectedVideoStreams.length === 0) {
      ReturnToParent();
      return;
    }

    const promises = [];

    for (const stream of selectedVideoStreams) {
      const data = {} as DeleteVideoStreamRequest;
      data.id = stream.id;
      promises.push(
        DeleteVideoStream(data).then(() => { }).catch(() => { })
      );
    }

    const p = Promise.all(promises);

    await p.then(() => {
      setInfoMessage('Delete Stream Successful');
    }).catch((error) => {
      setInfoMessage('Delete Stream Error: ' + error.message);
    });

  }

  const getTotalCount = useMemo(() => {
    if (props.overrideTotalRecords !== undefined) {
      return props.overrideTotalRecords;
    }

    return selectedVideoStreams.length;

  }, [props.overrideTotalRecords, selectedVideoStreams]);

  if (props.skipOverLayer === true) {
    return (
      <DeleteButton disabled={getTotalCount === 0 || selectedVideoStreams[0].isUserCreated !== true} iconFilled={props.iconFilled} onClick={async () => await deleteVideoStream()} tooltip="Delete Stream" />
    );
  }

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header={`Delete ${getTotalCount < 2 ? getTotalCount + ' Stream ?' : getTotalCount + ' Streams ?'}`}
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={6}
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

      <Button
        disabled={(selectedVideoStreams === undefined || selectedVideoStreams.length === 0) || (!props.value?.isUserCreated)}
        icon="pi pi-minus"
        onClick={() => setShowOverlay(true)}
        rounded
        severity="danger"
        size="small"
        text={props.iconFilled !== true}
        tooltip="Delete Stream"
        tooltipOptions={getTopToolOptions}
      />

    </>
  );
}

VideoStreamDeleteDialog.displayName = 'VideoStreamDeleteDialog';

type VideoStreamDeleteDialogProps = {
  iconFilled?: boolean | undefined;
  onClose?: (() => void);
  overrideTotalRecords?: number | undefined;
  skipOverLayer?: boolean | undefined;
  value?: VideoStreamDto | undefined;
  values?: VideoStreamDto[] | undefined;
};

export default memo(VideoStreamDeleteDialog);
