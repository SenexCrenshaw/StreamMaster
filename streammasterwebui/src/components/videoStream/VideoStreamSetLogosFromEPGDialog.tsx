/* eslint-disable @typescript-eslint/no-unused-vars */
import { useState, useCallback, memo, useMemo } from "react";
import { type VideoStreamsSetVideoStreamsLogoFromEpgApiArg } from "../../store/iptvApi";
import { type VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg } from "../../store/iptvApi";
import { useVideoStreamsSetVideoStreamChannelNumbersFromParametersMutation, useVideoStreamsSetVideoStreamsLogoFromEpgFromParametersMutation, type VideoStreamsSetVideoStreamChannelNumbersFromParametersApiArg, useVideoStreamsSetVideoStreamsLogoFromEpgMutation } from "../../store/iptvApi";
import { type VideoStreamDto } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import ImageButton from "../buttons/ImageButton";
import OKButton from "../buttons/OKButton";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useSortInfo } from "../../app/slices/useSortInfo";


type VideoStreamSetLogosFromEPGDialogProps = {
  id: string;
  values: VideoStreamDto[];
};


const VideoStreamSetLogosFromEPGDialog = ({ id, values }: VideoStreamSetLogosFromEPGDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [block, setBlock] = useState<boolean>(false);

  const [videoStreamsSetVideoStreamsLogoFromEpgFromParametersMutation] = useVideoStreamsSetVideoStreamsLogoFromEpgFromParametersMutation();
  const [videoStreamsSetVideoStreamsLogoFromEpgMutation] = useVideoStreamsSetVideoStreamsLogoFromEpgMutation();

  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);
  const { sortInfo } = useSortInfo(id);

  const ReturnToParent = () => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  };

  const onSetLogoSave = useCallback(async () => {
    setBlock(true);
    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();
        return;
      }

      const toSendAll = {} as VideoStreamsSetVideoStreamsLogoFromEpgFromParametersApiArg;
      toSendAll.parameters = queryFilter;


      videoStreamsSetVideoStreamsLogoFromEpgFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Set Streams Successfully');
        }
        ).catch((error) => {
          setInfoMessage('Set Streams Error: ' + error.message);
        });
      return;
    }

    const ids = [...new Set(values.map((item: VideoStreamDto) => item.id))] as string[];

    const toSend = {} as VideoStreamsSetVideoStreamsLogoFromEpgApiArg;
    toSend.ids = ids;
    toSend.orderBy = sortInfo.orderBy;

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
        videoStreamsSetVideoStreamsLogoFromEpgMutation(toSend)
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


  }, [queryFilter, selectAll, sortInfo.orderBy, values, videoStreamsSetVideoStreamsLogoFromEpgFromParametersMutation, videoStreamsSetVideoStreamsLogoFromEpgMutation]);

  const getTotalCount = useMemo(() => {

    return values.length;

  }, [values]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        header="Match Logos"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className='flex flex-column mt-2 col-6'>
            {`Match (${getTotalCount}) video stream logo${getTotalCount > 1 ? 's' : ''} to ${getTotalCount > 1 ? 'their' : 'its'} EPG logo${getTotalCount > 1 ? 's' : ''}?'`}
          </div>
          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onSetLogoSave()} />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <ImageButton disabled={getTotalCount === 0 && !selectAll} onClick={() => setShowOverlay(true)} tooltip='Set Logo from EPG Streams' />

    </>
  )
}

VideoStreamSetLogosFromEPGDialog.displayName = 'VideoStreamSetLogosFromEPGDialog';

export default memo(VideoStreamSetLogosFromEPGDialog);
