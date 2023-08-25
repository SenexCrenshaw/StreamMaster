
import { useState, useCallback, memo, useMemo } from "react";

import { type VideoStreamDto, type SetVideoStreamSetEpGsFromNameRequest } from "../../store/iptvApi";
import { SetVideoStreamSetEPGsFromName } from "../../store/signlar_functions";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import BookButton from "../buttons/BookButton";
import OKButton from "../buttons/OKButton";



const VideoStreamSetEPGsFromNameDialog = (props: VideoStreamSetEPGsFromNameDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [block, setBlock] = useState<boolean>(false);


  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  }, []);


  const onChangeEPG = useCallback(async () => {
    setBlock(true);


    const ids = [...new Set(props.values.map((item: VideoStreamDto) => item.id))] as string[];

    const toSend = {} as SetVideoStreamSetEpGsFromNameRequest;
    const max = 500;

    let count = 0;

    const promises = [];

    while (count < ids.length) {
      if (count + max < ids.length) {
        toSend.ids = ids.slice(count, count + max);
      } else {
        toSend.ids = ids.slice(count, ids.length);
      }

      count += max;
      promises.push(
        SetVideoStreamSetEPGsFromName(toSend)
          .then(() => {
          }).catch(() => { })
      );

    }

    const p = Promise.all(promises);

    await p.then(() => {
      setInfoMessage('Successful');
    }).catch((error) => {
      setInfoMessage('Error: ' + error.message);
    });


  }, [props.values]);

  const getTotalCount = useMemo(() => {
    if (props.overrideTotalRecords !== undefined) {
      return props.overrideTotalRecords;
    }

    return props.values.length;

  }, [props.overrideTotalRecords, props.values.length]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        header="Match EPGs"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className='flex flex-column mt-2 col-6'>
            {`Match (${getTotalCount}) video stream EPG${getTotalCount > 1 ? 's' : ''} to ${getTotalCount > 1 ? 'their' : 'its'} EPG logo${getTotalCount > 1 ? 's' : ''}?'`}
          </div>

          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onChangeEPG()} />

          </div>
        </div>
      </InfoMessageOverLayDialog>

      <BookButton disabled={getTotalCount === 0} onClick={() => setShowOverlay(true)} tooltip={`Set EPG from Name for (${props.values.length}) Streams`} />

    </>
  );
}

VideoStreamSetEPGsFromNameDialog.displayName = 'VideoStreamSetEPGsFromNameDialog';
VideoStreamSetEPGsFromNameDialog.defaultProps = {
}

type VideoStreamSetEPGsFromNameDialogProps = {
  overrideTotalRecords?: number | undefined;
  values: VideoStreamDto[];
};

export default memo(VideoStreamSetEPGsFromNameDialog);
