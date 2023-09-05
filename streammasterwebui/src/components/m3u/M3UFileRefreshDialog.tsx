import { useState, useCallback, memo } from "react";
import { type M3UFilesRefreshM3UFileApiArg } from "../../store/iptvApi";
import { type M3UFileDto, useM3UFilesRefreshM3UFileMutation } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import RefreshButton from "../buttons/RefreshButton";
import OKButton from "../buttons/OKButton";

const M3UFileRefreshDialog = (props: M3UFileRefreshDialogProps) => {

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [m3uFilesRefreshM3UFileMutation] = useM3UFilesRefreshM3UFileMutation();

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
    const tosend = {} as M3UFilesRefreshM3UFileApiArg;

    tosend.id = props.selectedFile.id;

    m3uFilesRefreshM3UFileMutation(tosend)
      .then(() => {
        setInfoMessage('M3U File Removed Successfully');
      }).catch((e) => {
        setInfoMessage('M3U File Removed Error: ' + e.message);
      });


  };


  return (
    <>

      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header='Refresh M3U File'
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
              <OKButton label="Refresh M3U File" onClick={async () => await refreshFile()} />
            </div>
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <RefreshButton
        onClick={() => setShowOverlay(true)}
        tooltip="Refresh M3U File"
      />


    </>
  );
}

M3UFileRefreshDialog.displayName = 'M3UFileRefreshDialog';

type M3UFileRefreshDialogProps = {
  selectedFile: M3UFileDto;
};

export default memo(M3UFileRefreshDialog);
