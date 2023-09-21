
import { Button } from "primereact/button";
import { memo, useCallback, useMemo, useState } from "react";
import { useQueryFilter } from "../../app/slices/useQueryFilter";
import { useSelectAll } from "../../app/slices/useSelectAll";
import { useSelectedVideoStreams } from "../../app/slices/useSelectedVideoStreams";
import { getTopToolOptions } from "../../common/common";
import { ResetLogoIcon } from "../../common/icons";
import { ReSetVideoStreamsLogo, ReSetVideoStreamsLogoFromParameters } from "../../smAPI/VideoStreams/VideoStreamsMutateAPI";
import { type ReSetVideoStreamsLogoRequest, type VideoStreamDto, type VideoStreamsReSetVideoStreamsLogoFromParametersApiArg } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import OKButton from "../buttons/OKButton";


type VideoStreamResetLogosDialogProps = {
  readonly id: string;
}

const VideoStreamResetLogosDialog = ({ id }: VideoStreamResetLogosDialogProps) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [block, setBlock] = useState<boolean>(false);
  const { selectAll } = useSelectAll(id);
  const { queryFilter } = useQueryFilter(id);

  const { selectedVideoStreams } = useSelectedVideoStreams(id);


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


      await ReSetVideoStreamsLogoFromParameters(toSendAll)
        .then(() => {
          setInfoMessage('Set Streams Successfully');
        }
        ).catch((error) => {
          setInfoMessage('Set Streams Error: ' + error.message);
        });

      return;
    }

    const ids = [...new Set(selectedVideoStreams.map((item: VideoStreamDto) => item.id))] as string[];

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
        ReSetVideoStreamsLogo(toSend)
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



  }, [queryFilter, selectAll, selectedVideoStreams]);

  const getTotalCount = useMemo(() => {

    return selectedVideoStreams.length;

  }, [selectedVideoStreams.length]);

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
        <div className="flex justify-content-center w-full align-items-center h-full">
          <OKButton label="Reset Logo" onClick={async () => await onSetLogoSave()} />

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
