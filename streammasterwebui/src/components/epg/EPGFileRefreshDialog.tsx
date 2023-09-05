import { useState, useCallback, memo } from "react";

import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import RefreshButton from "../buttons/RefreshButton";
import OKButton from "../buttons/OKButton";
import { type EpgFilesDto } from "../../store/iptvApi";
import { type EpgFilesRefreshEpgFileApiArg } from "../../store/iptvApi";
import { useEpgFilesRefreshEpgFileMutation } from "../../store/iptvApi";

const EPGFileRefreshDialog = (props: EPGFileRefreshDialogProps) => {

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [epgFilesRefreshEpgFileMutation] = useEpgFilesRefreshEpgFileMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  }, []);

  const refreshFile = async () => {

    if (!props.selectedFile) {
      return;
    }

    setBlock(true);
    const tosend = {} as EpgFilesRefreshEpgFileApiArg;

    tosend.id = props.selectedFile.id;

    epgFilesRefreshEpgFileMutation(tosend)
      .then(() => {
        setInfoMessage('EPG File Removed Successfully');
      }).catch((e) => {
        setInfoMessage('EPG File Removed Error: ' + e.message);
      });


  };


  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header='Refresh EPG File'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className='m-0 p-0 border-1 border-round surface-border'>
          <div className='m-3'>
            <h3 />
            <div className="card flex mt-3 flex-wrap gap-2 justify-content-center">
              <OKButton label="Refresh EPG File" onClick={async () => await refreshFile()} />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <RefreshButton
        onClick={() => setShowOverlay(true)}
        tooltip="Refresh EPG File"
      />


    </>
  );
}

EPGFileRefreshDialog.displayName = 'EPGFileRefreshDialog';

type EPGFileRefreshDialogProps = {
  selectedFile: EpgFilesDto;
};

export default memo(EPGFileRefreshDialog);
