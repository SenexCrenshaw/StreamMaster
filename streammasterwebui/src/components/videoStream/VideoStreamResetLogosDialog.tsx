
import { useState, useCallback, memo, useMemo } from "react";
import { getTopToolOptions } from "../../common/common";
import { ResetLogoIcon } from "../../common/icons";
import { type VideoStreamsReSetVideoStreamsLogoFromParametersApiArg } from "../../store/iptvApi";
import { type VideoStreamDto, type ReSetVideoStreamsLogoRequest, useVideoStreamsReSetVideoStreamsLogoMutation, useVideoStreamsReSetVideoStreamsLogoFromParametersMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import OKButton from "../buttons/OKButton";
import { Button } from "primereact/button";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useQueryFilter } from "../../app/slices/useQueryFilter";


type VideoStreamResetLogosDialogProps = {
  readonly id: string;
  readonly values: VideoStreamDto[];
}

const VideoStreamResetLogosDialog = ({ id, values }: VideoStreamResetLogosDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [block, setBlock] = useState<boolean>(false);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const [videoStreamsReSetVideoStreamsLogoMutation] = useVideoStreamsReSetVideoStreamsLogoMutation();
  const [videoStreamsReSetVideoStreamsLogoFromParametersMutation] = useVideoStreamsReSetVideoStreamsLogoFromParametersMutation();

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

      const toSendAll = {} as VideoStreamsReSetVideoStreamsLogoFromParametersApiArg;

      toSendAll.parameters = queryFilter;


      videoStreamsReSetVideoStreamsLogoFromParametersMutation(toSendAll)
        .then(() => {
          setInfoMessage('Set Streams Successfully');
        }
        ).catch((error) => {
          setInfoMessage('Set Streams Error: ' + error.message);
        });

      return;
    }

    const ids = [...new Set(values.map((item: VideoStreamDto) => item.id))] as string[];

    const toSend = {} as ReSetVideoStreamsLogoRequest;
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
        videoStreamsReSetVideoStreamsLogoMutation(toSend)
          .then(() => {

          }).catch(() => { })
      );

    }

    const p = Promise.all(promises);

    await p.then(() => {
      setInfoMessage('Successful');
      // onChange?.(ret);
    }).catch((error) => {
      setInfoMessage('Error: ' + error.message);
    });



  }, [queryFilter, selectAll, values, videoStreamsReSetVideoStreamsLogoFromParametersMutation, videoStreamsReSetVideoStreamsLogoMutation]);

  const getTotalCount = useMemo(() => {

    return values.length;

  }, [values.length]);

  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Reset Logo to original"
        infoMessage={infoMessage}
        onClose={() => { ReturnToParent(); }}
        overlayColSize={4}
        show={showOverlay}
      >
        <div className="border-1 surface-border flex grid flex-wrap justify-content-center p-0 m-0">
          <div className='flex flex-column mt-2 col-6'>
            Reset logos?
          </div>
          <div className="flex col-12 gap-2 mt-4 justify-content-center ">
            <OKButton onClick={async () => await onSetLogoSave()} />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <Button
        disabled={getTotalCount === 0 && !selectAll}
        icon={<ResetLogoIcon sx={{ fontSize: 18 }} />}
        onClick={() => setShowOverlay(true)}
        rounded
        size="small"
        tooltip='Set Logo from EPG for Streams'
        tooltipOptions={getTopToolOptions}
      />

    </>
  )
}

VideoStreamResetLogosDialog.displayName = 'Auto Set Channel Numbers';

export default memo(VideoStreamResetLogosDialog);
